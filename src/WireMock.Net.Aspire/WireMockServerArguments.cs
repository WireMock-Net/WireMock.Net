using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Represents the arguments required to configure and start a WireMock Server.
/// </summary>
public class WireMockServerArguments
{
    private const int DefaultExternalPort = 9091;

    public int Port { get; set; } = DefaultExternalPort;

    public string? AdminUsername { get; set; }

    public string? AdminPassword { get; set; }

    /// <summary>
    /// Defines if the static mappings should be read at startup.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool ReadStaticMappings { get; set; }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool WithWatchStaticMappings { get; set; }

    /// <summary>
    /// Specifies the path for the (static) mapping json files.
    /// </summary>
    public string? MappingsPath { get; set; }

    /// <summary>
    /// Indicates whether the server should use basic authentication.
    /// </summary>
    public bool HasBasicAuthentication => !string.IsNullOrEmpty(AdminUsername) && !string.IsNullOrEmpty(AdminPassword);

    /// <summary>
    /// Converts the current instance's properties to an array of command-line arguments for starting the Temporal server.
    /// </summary>
    /// <returns>An array of strings representing the command-line arguments.</returns>
    public string[] GetArgs()
    {
        var result = new List<string>();

        if (HasBasicAuthentication)
        {
            AddAlways(result, "--AdminUserName", AdminUsername!);
            AddAlways(result, "--AdminPassword", AdminPassword!);
        }
        
        AddIfTrue(result, "--ReadStaticMappings", ReadStaticMappings);
    
        if (WithWatchStaticMappings)
        {
            AddAlways(result, "--WatchStaticMappings", "true");
            AddAlways(result, "--WatchStaticMappingsInSubdirectories", "true");
        }

        return result.ToArray();
    }

    private static void AddIfNotNull(ICollection<string> list, string argument, string? value)
    {
        if (value is not null)
        {
            list.Add(argument);
            list.Add(value);
        }
    }

    private static void AddAlways(ICollection<string> list, string argument, string value)
    {
        list.Add(argument);
        list.Add(value);
    }

    private static void AddIfTrue(ICollection<string> list, string argument, bool value)
    {
        if (value)
        {
            list.Add(argument);
            list.Add("true");
        }
    }
}