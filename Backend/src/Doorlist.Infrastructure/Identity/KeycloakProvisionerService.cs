namespace Doorlist.Infrastructure.Identity;

using Doorlist.Application.Identity;
using Doorlist.Domain.Utility;

using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options;

/// <summary>
/// Administrates the Keycloak doorlist-api client.
/// </summary>
public class KeycloakProvisionerService : IIdentityProvisionerService
{
    private readonly ILogger<KeycloakProvisionerService> _logger;
    private readonly IKeycloakUserClient _keycloakUserClient;
    private readonly KeycloakSettings _keycloakSettings;

    public string ProviderName => "Keycloak";
    public string Issuer => _keycloakSettings.Issuer;

    public KeycloakProvisionerService(
        ILogger<KeycloakProvisionerService> logger, 
        IKeycloakUserClient keycloakUserClient, 
        IOptions<KeycloakSettings> keycloakSettingsOptions)
    {
        _logger = logger;
        _keycloakUserClient = keycloakUserClient;
        _keycloakSettings = keycloakSettingsOptions.Value;
    }
    
    /// <summary>
    /// Register a user with Keycloak.
    /// </summary>
    /// <returns>The Subject Identifier (sub)</returns>
    public async Task<Result<string>> CreateUserAsync(string email, string password, Guid doorlistUserId, CancellationToken cancellationToken)
    {
        UserRepresentation userRepresentation = new UserRepresentation
        {
            Email = email,
            Username = email,
            Enabled = true,
            Credentials = [ new CredentialRepresentation() { Type = "password", Value = password, Temporary = false } ],
            Attributes = new Dictionary<string, ICollection<string>>
            {
                { "doorlist_user_id", new List<string>(){ doorlistUserId.ToString() } } // Store the GUID in Keycloak.
            }
        };
        
        // Attempt registration with Keycloak.
        try
        {
            var kcResponse = await _keycloakUserClient.CreateUserWithResponseAsync(_keycloakSettings.Realm, userRepresentation, cancellationToken);
            if (kcResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogWarning("Keycloak rejected user creation for {Email} - already exists", email);
                return Result<string>.Failure("A user with this email already exists", ErrorType.Conflict);
            }
            if (!kcResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Keycloak rejected user creation - HTTP: {Code}, Request Message: {RequestMessage}, Response Message: {ResponseMessage}", 
                    kcResponse.StatusCode, kcResponse.RequestMessage, kcResponse.ReasonPhrase);
                return Result<string>.Failure("Keycloak rejected user creation", ErrorType.DependencyFailure);
            }

            var keycloakSubId = kcResponse.Headers.Location?.Segments.Last().Trim('/');
            if (string.IsNullOrEmpty(keycloakSubId))
            {
                _logger.LogError("Keycloak created the user {Email} but didn't return a valid sub", email);
                return Result<string>.Failure("Keycloak returned a malformed user on creation", ErrorType.DependencyFailure);
            }
            return Result<string>.Success(keycloakSubId ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Keycloak service unreachable: {Message}", ex.Message);
            return Result<string>.Failure("Keycloak service unreachable", ErrorType.DependencyFailure);
        }
    }
    
    /// <summary>
    /// Delete a user in Keycloak.
    /// </summary>
    public async Task<Result<string>> DeleteUserAsync(string externalId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _keycloakUserClient.DeleteUserWithResponseAsync(_keycloakSettings.Realm, externalId, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to delete Keycloak user {Sub} with code [{StatusCode}]", externalId, response.StatusCode);
                return Result<string>.Failure("Identity deletion failed", ErrorType.DependencyFailure);
            }
            return Result<string>.Success("Keycloak user successfully deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to Keycloak for deleting user {Sub}", externalId);
            return Result<string>.Failure("Identity service unreachable", ErrorType.DependencyFailure);
        }
    }
}