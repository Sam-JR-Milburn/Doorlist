namespace Doorlist.Infrastructure.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Runtime.InteropServices;

/**
 * PostQuantumStartupService:
 * Checks OpenSSL (Kestrel default to it) for PQC availability and attempts to configure it
 * if the version matches. 
 */
public class PostQuantumStartupService : IHostedService
{
    private ILogger<PostQuantumStartupService> _logger;
    private IConfiguration _configuration; 

    public PostQuantumStartupService(IConfiguration configuration, ILogger<PostQuantumStartupService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    /// <summary>
    /// OpenSSL version >= 3.5.0 (Apr 8 2025) introduces support for hybrid post-quantum key exchange.
    /// This will allow the Kestrel web server to use Classic X25519 ECDHE with the NIST-standardized ML-KEM-768 for post-quantum cryptography. 
    /// </summary>
    private static readonly Version MinimumOpenSslVersion = new Version(3, 5, 0);

    /// <summary>
    /// Generic, but runs 'openssl version' for the startup service.
    /// </summary>
    /// <returns>
    /// Success: whether the command worked, Output: the string result of the command + args
    /// </returns>
    private static async Task<(bool Success, string Output)> RunCommandAsync(string command, string arguments, CancellationToken cancellationToken)
    {
        try
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
                await process.WaitForExitAsync(cancellationToken);
                return (process.ExitCode == 0, output.Trim()); // Check for command success
            }
        }
        catch 
        {
            return (false, String.Empty);
        }
    }
    
    /// <summary>
    /// A string parser for the OpenSSL output. 
    /// </summary>
    /// <returns>The OpenSSL version as a Version object, should openssl version work</returns>
    private async Task<Version?> GetOpenSslVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await RunCommandAsync("openssl", "version", cancellationToken);
            if (!result.Success) return null;
            
            string[] tokens = result.Output.Split(' '); // OpenSSL 3.5.5 27 Jan 2026 (Library: OpenSSL 3.5.5 27 Jan 2026)
            if (tokens.Length < 2) return null; // 2: Covering the first space sep tokens above. 
            
            return Version.TryParse(tokens[1], out var version) ? version : null;
        }
        catch (Exception e)
        {
            _logger.LogDebug(e, "Failure in detecting the system OpenSSL version");
            return null;
        }
    }
    
    /// <summary>
    /// Rudimentary algorithm list check for ML-KEM availability.
    /// </summary>
    private async Task<bool> VerifyMlKemAvailabilityAsync(CancellationToken cancellationToken)
    {
        var result = await RunCommandAsync("openssl", "list -kem-algorithms", cancellationToken);
        if (!result.Success)
        {
            _logger.LogWarning("Could not verify ML-KEM algorithm availability.");
            return false;
        }
        
        // 1. Check for the algorithm in the output: primitives available.
        if (result.Output.Contains("ML-KEM", StringComparison.OrdinalIgnoreCase) || 
            result.Output.Contains("mlkem", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("ML-KEM algorithms confirmed to be available in OpenSSL.");
        }
        else
        {
            _logger.LogInformation("ML-KEM algorithms not confirmed to be available in OpenSSL.");
            return false;
        }
        
        // 2. Check for the 'groups' config: protocol actually enabled.
        var groupsResult = await RunCommandAsync("openssl", "list -tls-groups", cancellationToken);
        if (groupsResult.Success)
        {
            string[] lines = groupsResult.Output.Split(':');
            foreach (string line in lines)
            {
                if (line.Contains("mlkem", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Set the OPENSSL_CONF to the config specified location.
    /// </summary>
    /// <returns> Whether the OpenSSL config could be set. </returns>
    private bool ConfigureOpensslConfigurationAsync()
    {
        string? pqcConfigPath = _configuration.GetValue<string>("OpenSSL:ConfigPath");
        if (pqcConfigPath is null || !File.Exists(pqcConfigPath))
        {
            _logger.LogError("Couldn't read alternative openssl.cnf file from configuration file.");
            return false;
        }
        
        // Copy to location, setup OPENSSL_CONF environment variable
        try
        {
            string? envConfigPath = Environment.GetEnvironmentVariable("OPENSSL_CONF");
            if (string.IsNullOrEmpty(envConfigPath) || !envConfigPath.Equals(pqcConfigPath, StringComparison.OrdinalIgnoreCase))
            {
                Environment.SetEnvironmentVariable("OPENSSL_CONF", pqcConfigPath);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception on reading or setting the OPENSSL_CONF environment variable.");
            return false;
        }
    }
    
    /// <summary>
    /// Runs the startup service to check for and configure PQC.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Post-Quantum Cryptography startup check");

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _logger.LogWarning("PQC TLS check is skipped - automatic OpenSSL configuration only supported on Linux.");
            return;
        }
        
        // Check version
        var openSslVersion = await GetOpenSslVersionAsync(cancellationToken);
        if (openSslVersion is null)
        {
            _logger.LogError("PQC TLS is unavailable. Startup service couldn't determine OpenSSL version. Is OpenSSL installed?");
            return;
        }
        if (openSslVersion < MinimumOpenSslVersion)
        {
            _logger.LogWarning("PQC TLS unavailable. OpenSSL version {Detected} detected. Minimum required is version {Minimum}.", openSslVersion.ToString(), MinimumOpenSslVersion.ToString());
            return;
        }
        
        // Set the custom OPENSSL_CONF
        if (!ConfigureOpensslConfigurationAsync())
        {
            _logger.LogError("OpenSSL configuration couldn't be modified to ensure PQC for TLS.");
            return;
        }

        // Verify algorithms list.
        if (! await VerifyMlKemAvailabilityAsync(cancellationToken))
        {
            _logger.LogWarning("PQC TLS is unavailable. OpenSSL is either lacking the ML-KEM cryptographic primitive or the groups config.");
            return;
        }
        
        _logger.LogInformation("PQC TLS enabled.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}