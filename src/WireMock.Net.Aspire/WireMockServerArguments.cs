using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Represents the arguments required to configure and start a WireMock.Net Server.
/// </summary>
public class WireMockServerArguments
{
    private const string DefaultLogger = "WireMockConsoleLogger";
    internal const int DefaultPort = 9091;

    /// <summary>
    /// The port where WireMock.Net is listening.
    ///
    /// Default value is <c>9091</c>.
    /// </summary>
    public int Port { get; set; } = DefaultPort;

    /// <summary>
    /// The admin username.
    /// </summary>
    public string? AdminUsername { get; set; }

    /// <summary>
    /// The admin password.
    /// </summary>
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
    /// Indicates whether the admin interface has Basic Authentication.
    /// </summary>
    public bool HasBasicAuthentication => !string.IsNullOrEmpty(AdminUsername) && !string.IsNullOrEmpty(AdminPassword);

    /// <summary>
    /// Converts the current instance's properties to an array of command-line arguments for starting the WireMock.Net server.
    /// </summary>
    /// <returns>An array of strings representing the command-line arguments.</returns>
    public string[] GetArgs()
    {
        var args = new Dictionary<string, string>();

        Add(args, "--WireMockLogger", DefaultLogger);

        if (HasBasicAuthentication)
        {
            Add(args, "--AdminUserName", AdminUsername!);
            Add(args, "--AdminPassword", AdminPassword!);
        }

        if (ReadStaticMappings)
        {
            Add(args, "--ReadStaticMappings", "true");
        }

        if (WithWatchStaticMappings)
        {
            Add(args, "--ReadStaticMappings", "true");
            Add(args, "--WatchStaticMappings", "true");
            Add(args, "--WatchStaticMappingsInSubdirectories", "true");
        }

        return args
            .SelectMany(k => new [] { k.Key, k.Value })
            .ToArray();
    }

    private static void Add(IDictionary<string, string> args, string argument, string value)
    {
        args[argument] = value;
    }
}