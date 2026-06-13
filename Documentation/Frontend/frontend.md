## Doorlist Frontend Documentation

<br />
<br />

### Language Choice, Overall Design

<br />
I've decided on NextJS with TypeScript. 

This is a market choice to cover a React/React-like framework, but I've had previous experience is in Svelte/TS and each project has it's niche.
I'll only find out through practice and study - I'm intending to use modern features and best practice within the React ecosystem. 

It does however present an interesting exercise in thinking like a web developer, in patterns that I'm not used to - functional programming, hooks, callbacks, etc. 

### Configuration 
At present, the configuration is fairly simple.

We set a system variable, <b>DOORLIST_FRONTEND_HTTP_PORT</b> to set the application port, with a 9080 fallback.

```json
{
  "...":  "...",
  "scripts": {
    "dev": "next dev -p ${DOORLIST_FRONTEND_HTTP_PORT:-9080}",
    "build": "next build",
    "start": "next start -p ${DOORLIST_FRONTEND_HTTP_PORT:-9080}",
    "lint": "eslint"
  }
}
```

NextJS is designed to run as an application server and not a full web server. 
Hence, HTTPS will be achieved by using Nginx as a proxy. 

In development, port 9443 HTTPS is mapped to port 9080 HTTP. This allows for several optimisations:
 - HTTPS encryption doesn't choke the single-threaded node processing. 
 - Static images can be mapped directly from the /public folder without requirement to have NodeJS serve them.
 - Ingress control, load balancing, etcetera.

### Security Frameworks

Keycloak requires an implementation of Proof Key for Code Exchange (PKCE) for login.

![text](../Res/auth-sequence-auth-code-pkce.png)

Generating the code verifier/code challenge and redirecting (Steps 2,3,4) takes place via a method call from the components/Auth/LoginRedirect.tsx module.
At step 2 the code challenge and an anti-CSRF token are persisted in session storage for the handshake lifecycle. 

The login button triggers a redirect to the secure Keycloak login portal (Steps 4,5) where authentication is established 
and the access token retrieved by presenting proof in steps 6 and 7 of the code that made that login valid.

At step 10, Keycloak passes a redirect URI as query param state (set in the initial redirect) that is received by the /auth/callback NextJS route. 

The auth callback route makes a final check to see that the anti-CSRF token is the same as the one passed by Keycloak, preventing malicious redirect.

PKCE here guarantees that the user exchanging the code for access is the user that requested it. 
The CSRF protections guarantee that the incoming redirect was initiated by user intention in the same browser session. 




















































































-----