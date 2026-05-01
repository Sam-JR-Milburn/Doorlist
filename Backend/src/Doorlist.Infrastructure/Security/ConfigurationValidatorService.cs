namespace Doorlist.Infrastructure.Security;

using Doorlist.Domain.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Checks the configuration for values and provides clear error messages for what's missing.
/// </summary>
public class ConfigurationValidatorService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationValidatorService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    public ConfigurationValidatorService(
        IConfiguration configuration, 
        ILogger<ConfigurationValidatorService> logger, 
        IHostApplicationLifetime applicationLifetime)
    {
        _configuration = configuration;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }
    
    // Validators
    
    /// <summary>
    /// Checks the key:value exists within the config context.
    /// </summary>
    private bool ValidateRequired(string key, List<string> failures)
    {
        var value = _configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            failures.Add(key);
            _logger.LogError($"Missing required configuration key: {key}");
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Checks whether a file can be read from config, and then read on disk by the current process.
    /// </summary>
    private bool ValidateFile(string key, string description, List<string> failures)
    {
        // File existence checks
        var filePath = _configuration[key];
        if (string.IsNullOrEmpty(filePath))
        {
            _logger.LogError($"Missing required {description} file path configuration key: {key}");
            failures.Add(key);
            return false;
        }
        var qualifiedPath = Path.GetFullPath(filePath);
        if (!File.Exists(qualifiedPath))
        {
            _logger.LogError($"{description.Capitalise()} file not found: {filePath} ({key})");
            failures.Add(key);
            return false;
        }
        // Check the file can be opened for at-least reading.
        try
        {
            using var stream = File.Open(qualifiedPath, FileMode.Open, FileAccess.Read);
            return true;
        }
        catch (UnauthorizedAccessException uae)
        {
            _logger.LogError($"{description.Capitalise()} file ({qualifiedPath}) unreadable by process: {uae.Message}");
        }
        catch (Exception e)
        {
            _logger.LogError($"Unknown error in reading {description} file: {e.Message}");
        }
        failures.Add(key);
        return false;
    }
    
    /// <summary>
    /// Checks whether a URL exists and checks that it's validly formed.
    /// </summary>
    private bool ValidateHttpUrl(string key, List<string> failures)
    {
        var value = _configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            _logger.LogError($"Missing required URL configuration key: {key}");
            failures.Add(key);
            return false;
        }
        // Parse attempt
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
        {
            _logger.LogError($"Invalid URL: {value} for key {key}");
            failures.Add(key);
            return false;
        }
        return true;
    }

    // Lifecycle
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking configuration for validity on startup.");
        List<string> failures = new List<string>();
        
        // Auth
        ValidateRequired("Keycloak:ClientSecret", failures);
        ValidateRequired("Kestrel:Endpoints:Https:Certificate:Password", failures);
        ValidateRequired("Databases:DoorlistAPI:Password", failures);
        
        // Files
        ValidateFile("Kestrel:Endpoints:Https:Certificate:Path", "HTTPS TLS cert", failures);
        ValidateFile("OpenSSL:ConfigPath", "OpenSSL config for PQC TLS", failures);
        
        // URLs
        ValidateHttpUrl("Keycloak:BaseUrl", failures);
        ValidateHttpUrl("Frontend:BaseUrl", failures);
        ValidateHttpUrl("Kestrel:Endpoints:Http:Url", failures);
        ValidateHttpUrl("Kestrel:Endpoints:Https:Url", failures);
        
        // Check validation failures
        if (failures.Count > 0)
        {
            _logger.LogError($"Configuration validation failed with {failures.Count} failures.");
            _applicationLifetime.StopApplication(); // Graceful shutdown.
        }
        else
        {
            _logger.LogInformation("Configuration validation succeeded.");
        }
    }
    
    public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;
}