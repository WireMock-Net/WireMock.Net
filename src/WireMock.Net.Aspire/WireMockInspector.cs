using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Aspire.Hosting.WireMock;

internal static class WireMockInspector
{
    /// <summary>
    /// Opens the WireMockInspector tool to inspect the WireMock server.
    /// </summary>
    /// <param name="wireMockUrl"></param>
    /// <param name="title"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>
    /// Copy of <see href="https://github.com/WireMock-Net/WireMockInspector/blob/main/src/WireMock.Net.Extensions.WireMockInspector/WireMockServerExtensions.cs" />
    /// without requestFilters and no call to WaitForExit() method in the process so it doesn't block the caller.
    /// </remarks>
    public static void Inspect(string wireMockUrl, [CallerMemberName] string title = "")
    {
        try
        {
            var arguments = $"attach --adminUrl {wireMockUrl} --autoLoad --instanceName \"{title}\"";

            Process.Start(new ProcessStartInfo
            {
                FileName = "wiremockinspector",
                Arguments = arguments,
                UseShellExecute = false
            });
        }
        catch (Exception e)
        {
            throw new InvalidOperationException
            (
                message: @"Cannot find installation of WireMockInspector.
Execute the following command to install WireMockInspector dotnet tool:
> dotnet tool install WireMockInspector --global --no-cache --ignore-failed-sources
To get more info please visit https://github.com/WireMock-Net/WireMockInspector",
                innerException: e
            );
        }
    }
}