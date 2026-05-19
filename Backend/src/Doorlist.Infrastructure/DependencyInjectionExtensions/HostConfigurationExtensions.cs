namespace Doorlist.Infrastructure.DependencyInjectionExtensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class HostConfigurationExtensions
{
    /// <summary>
    /// Utility: Find the backend root path, containing 'Doorlist.slnx'.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">If the backend root path cannot be found.</exception>
    private static string GetBackendRootPath()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        const string anchor = "Doorlist.slnx";
        
        // Recursively look for the current dir containing Doorlist.slnx. Will stop at '/'.
        while (currentDirectory != null && currentDirectory.Parent != null)
        {
            if (currentDirectory.GetFiles(anchor).Any())
            {
                return currentDirectory.FullName;
            }
            currentDirectory = currentDirectory.Parent;
        }
        throw new DirectoryNotFoundException("Couldn't find the backend root.");
    }
    
    /// <summary>
    /// Utility: Prepend a relative path with a specified root path.
    /// </summary>
    private static void ConvertRelativePathToAbsolutePath(IConfiguration configuration, string key, string root)
    {
        var relativePath = configuration[key];
        if (!string.IsNullOrEmpty(relativePath) && !Path.IsPathRooted(relativePath))
        {
            configuration[key] = Path.GetFullPath(Path.Combine(root, relativePath));
        }
    }

    /// <summary>
    /// Service: Convert relative > absolute file config
    /// </summary>
    public static IServiceCollection NormaliseDevelopmentPaths(this IServiceCollection services, IConfiguration configuration)
    {
        // Normalise config before startup services.
        // If this throws an exception it will kill the program gracefully before app.Run().
        try
        {
            var rootPath = GetBackendRootPath();
            Console.WriteLine("Infrastructure - Found backend root path: " + rootPath);

            ConvertRelativePathToAbsolutePath(configuration, "Kestrel:Endpoints:Https:Certificate:Path", rootPath);
            ConvertRelativePathToAbsolutePath(configuration, "OpenSSL:ConfigPath", rootPath);
        }
        catch (DirectoryNotFoundException dnfe)
        {
            Console.WriteLine("Infrastructure - Couldn't find the root path: " + dnfe.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Infrastructure - Generic error in finding the root path: "+ex.Message);
            throw;
        }
        return services;
    }
}