// Copyright © WireMock.Net

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace WireMock.Net.Aspire.Tests;

[ExcludeFromCodeCoverage]
internal static class DockerUtils
{
    public static Lazy<bool> IsDockerRunningLinuxContainerMode => new(() => IsDockerRunning() && IsLinuxContainerMode());

    private static bool IsDockerRunning()
    {
        try
        {
            var processInfo = new ProcessStartInfo("docker", "info")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processInfo);
            process?.WaitForExit();
            return process?.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking Docker status: {ex.Message}");
            return false;
        }
    }

    private static bool IsLinuxContainerMode()
    {
        try
        {
            var processInfo = new ProcessStartInfo("docker", "version --format '{{.Server.Os}}'")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processInfo);
            var output = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();

            return output?.Contains("linux", StringComparison.OrdinalIgnoreCase) == true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking Docker container mode: {ex.Message}");
            return false;
        }
    }
}