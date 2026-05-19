## Doorlist Backend Documentation

<br />
<br />

### Language Choice, Overall Design

<br />
I have decided on C#/.NET as a language leveraging EF Core for the web API. 

The overall design is a classic Domain-Driven-Design and Clean Code architecture against four distinct service layers: Presentation, Application, Domain and Infrastructure. 

In particular I've chosen a "fake ticketmaster" project because it presents an interesting data engineering challenge that I haven't tackled through academic assignments or in my work so far.

This is exemplified by the classic problem: 100 people are trying to buy Aerosmith tickets. There are 25 Aerosmith tickets left. 
Everyone clicks it at roughly the same time - how do you portion 25 tickets? 

Users implies an identity/authentication system. I'm solving this with OAuth2.0 using internal Keycloak and external providers.  

Adding a ticket purchase to cart, implies an authorisation system. This will be tied to the User profile, and hasn't been formulated yet.
<br />
<br />
<br />

### Identity & Authentication System


#### Domain
I've developed an entity model where <b><i>User</i></b> entities are distinct from their authentication.  
User entity objects hold reference to a collection of identity providers. 

The <b><i>UserLogin</i></b> has: 
 - A <b>UserId</b> which is a foreign key reference to the GUIDv7 identifier of a User object, per database rules it's always valid.
 - A <b>ProviderName</b>, which is mostly self-descriptive - "Keycloak", "Google" and "Microsoft" are expected but won't be typed.
 - A <b>ProviderKey</b> - the 'sub' element of the token, which is the identifier for the user in that external system.
 - An <b>Issuer</b>, reflecting the 'iss' token, which is either a tenant (eg. a private company with a Microsoft system) or the default Issuer for that Provider - befitting a personal Google Account. 

The idea here is that one user can register with a Keycloak account, sign-in with Google, sign-in with Microsoft, but they can't sign-in with two Microsoft accounts. 

<u>The primary key is based on the UserId and the ProviderName</u> - User A can't have two UserLogin records with Microsoft. 

#### Identity Architecture

Authentication is done in Doorlist via OAuth2.0. I'm registering and configuring a definite number of external auth providers in my Program.cs and one internal service through Keycloak.

In particular the internal providers only leverage user registration and administration. Login is done through a redirecting provider prompt and not via direct grants. 

In the spirit of clean-code, I've built an interface abstraction, IIdentityProvisioner, that could implement this administration aspect for other identity providers, such as Authentik, Zitadel or FusionAuth. 
```C#
namespace Doorlist.Application.Identity;    

using Doorlist.Domain.Utility;

public interface IIdentityProvisionerService
{
    string ProviderName { get; }
    string Issuer { get; }
    Task<Result<string>> CreateUserAsync(string identifier, string password, Guid doorlistUserId, CancellationToken cancellationToken);
    Task<Result<string>> DeleteUserAsync(string externalId, CancellationToken cancellationToken);
}
```

#### Outbound Keycloak Admin Client

I've conformed my appsettings config to align with the formal schema expected by the Keycloak SDK library.  

```
"Keycloak": {
    "AuthServerUrl": "https://localhost:8443",
    "Realm": "doorlist",
    "VerifySsl": "false",
    "Resource": "doorlist-api",
    "credentials": {}
  },
```

This is ingested by a KeycloakSettings class that I can use at infrastructure DI runtime to configure the admin client and to build the KeycloakProvisionerService. 
```C#
namespace Doorlist.Infrastructure.Identity.Options;

using System.Collections;
using System.Reflection;
using System.Text;

/// <summary>
/// Maps the settings from the Keycloak config.
/// </summary>
public class KeycloakSettings
{
    public const string SectionName = "Keycloak";
    
    public string AuthServerUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    
    public string SslRequired { get; set; } = string.Empty;
    public bool VerifySsl { get; set; }
    public string Resource { get; set; } = string.Empty;
    
    public Dictionary<string, string> Credentials { get; set; } = []; 
    public string Issuer => $"{AuthServerUrl.TrimEnd('/')}/realms/{Realm}";

    public string Secret => Credentials.TryGetValue("secret", out var secret) ? secret : string.Empty;
    
    //...
}
```

This design means I can dynamically reconfigure the admin client if the realm details change - the client or realm name, or the issuer - which will change between production and development.

You'll notice in the following snippet an implementation of an OAuth 2.0 Client Credentials Grant system. 

```C#
// Keycloak: Use the Duende OpenID Connect library for token management
services.AddDistributedMemoryCache();
services.AddClientCredentialsTokenManagement().AddClient("keycloak.admin.token", client =>
{
    client.TokenEndpoint    = new Uri($"{keycloakSettings.AuthServerUrl}/realms/{keycloakSettings.Realm}/protocol/openid-connect/token");
    client.ClientId         = ClientId.Parse(keycloakSettings.Resource); 
    client.ClientSecret     = ClientSecret.Parse(keycloakSettings.Secret);
});

// Keycloak: Configure the outbound admin client
services.AddKeycloakAdminHttpClient(options => /* ... */)
.AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse("keycloak.admin.token"));
```

I've configured the Duende.AccessTokenManagement library to act as an HTTP middleware interceptor for outbound requests to the Keycloak API. 
This injects an 'Authorization: Bearer' header with the fresh access token for the admin client alone.

Notably, the distributed memory cache is configured to store a valid Keycloak 'doorlist-api' access JWT, and Duende will refresh that token if the delegating handler reads that the access JWT is expired or revoked.



<br />
<br />

### Security-first Development

#### Certs, Keys and Secrets

I've paid great attention to developing and deploying Doorlist in a secure way. 
This is not only in-order to not leak or expose production environment secrets to the repo, but to leverage modern authZ and authN protocols and technologies.

You'll notice roughly the same .gitignore in the Backend and the Infrastructure directories: 
```
# Infrastructure
# Deny-By-Default
**/*.key
**/*.pfx
**/*.p12
**/*.pem
**/*.csr
**/*.crt
**/*.env

# Allow Specific Dev Artifacts
!**/doorlist.api.dev.key
!**/doorlist.api.dev.crt
!**/doorlist.api.dev.pfx
!**/doorlist.keycloak.dev.key
!**/doorlist.keycloak.dev.crt
!**/doorlist.ca.dev.crt
!dev.env
```

This excludes all common cryptography artifacts with exact whitelisting for development certificates, keys, etc. 
Why? 
 - There is no chance of accidentally committing a production security artifact. 
 - By committing development artifacts, you can observe how the application would secure itself in practice. 

You'll notice in the [appsettings.Development.json](../../Backend/src/Doorlist.Presentation/appsettings.Development.json) that there's no "Keycloak:credentials:secret", "Databases:DoorlistAPI:Password" etc specified. 

In development mode, these are dotnet user-secrets local to the Presentation layer and available at runtime. 
In production, it's intended to be via Azure Key Vault, or some other solution. 

Note: in the infrastructure DI class you'll read that path names are normalised with respect to the project. 

#### Security Frameworks

By using OAuth2.0 JWTs from known providers, I can enforce time-limited and resource-limited application access using peer-reviewed standards and implementations.

You'll notice in the [Keycloak realm export](../../Infrastructure/Keycloak/realm-export.keycloak.dev.json) that the frontend Doorlist client has been setup at least in-principle for DPoP, which is an intended feature to strongly mitigate token theft.

DPoP is an OAuth2.0 extension laid out in RFC 9449 to sender-constrain tokens with a per-request proof to strongly mitigate token theft. 
It isn't implemented yet, but I'm planning to. 

When I do, I'll need to write a DPoP middleware pipeline carefully and to-spec against the RFC which will at least be a fun exercise. I'll try to test the implementation robustly for correctness.

Another item that I'm interested in implementing is a memory-cache (likely Redis) to blacklist a JWT token by it's nonce and enforce non-access without a database policy read (eg. Casbin). 

This is supposed to deal with when someone abuses account access, enforcing an immediate ban against a JWT that in-principle provides account access for up to an hour.
It's an especially valuable pattern in a federated system, which is an area I'm interested in. 

<br />
<br />

### High-Concurrency and Inventory Contention

While I haven't built the actual application logic around ticketing yet, the raison d'être for this project is to grapple fundamentally with the data engineering challenge inherent to the field. 

I'll be grappling with questions of pessimistic or optimistic locking, and the implementation of background services to prevent tickets from being scalped. 

<br />
<br />

### Startup Services

TODO: Discuss PQC and Configuration Validator startup services

<br />
<br />

### Transaction Discipline

TODO: Discuss how the user registration process preserves entity lifecycle appropriately with two dissimilar infrastructure services - the DB context and the Keycloak service.  