// Copyright © WireMock.Net

using System.Runtime.InteropServices;
using Newtonsoft.Json;
using WireMock.Net.Testcontainers;

namespace WireMock.Net.TestcontainersExample;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var original = Console.ForegroundColor;

        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Linux");
            await TestAsync("sheyenrath/wiremock.net:1.6.4");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = original;
        }

        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Linux Alpine");
            await TestAsync("sheyenrath/wiremock.net-alpine:1.6.4");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = original;
        }

        try
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("WithLinux");
            await TestAsync("WithLinux");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = original;
        }

        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Windows");
            await TestAsync("sheyenrath/wiremock.net-windows:1.6.4");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = original;
        }

        try
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("WithWindows");
            await TestAsync("WithWindows");
            await Task.Delay(1_000);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.ForegroundColor = original;
        }

        try
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Automatic");
            await TestAsync();
        }
        finally
        {
            Console.ForegroundColor = original;
        }
    }

    private static async Task TestAsync(string? image = null)
    {
        var builder = new WireMockContainerBuilder()
            .WithAdminUserNameAndPassword("x", "y")
            .WithWatchStaticMappings(true)
            .WithAutoRemove(true)
            .WithCleanUp(true);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            builder = builder.WithMappings(@"C:\Dev\GitHub\WireMock.Net\examples\WireMock.Net.Console.NET6\__admin\mappings");
        }
        else
        {
            builder = builder.WithMappings("/workspaces/WireMock.Net/examples/WireMock.Net.Console.NET6/__admin/mappings");
        }

        if (image != null)
        {
            builder = image switch
            {
                "WithWindows" => builder.WithWindowsImage(),
                "WithLinux" => builder.WithLinuxImage(),
                _ => builder.WithImage(image)
            };
        }

        var container = builder.Build();

        await container.StartAsync();

        await Task.Delay(1_000);

        var logs = await container.GetLogsAsync(DateTime.Now.AddDays(-1));
        Console.WriteLine("logs = " + logs.Stdout);

        var restEaseApiClient = container.CreateWireMockAdminClient();

        var settings = await restEaseApiClient.GetSettingsAsync();
        Console.WriteLine("settings = " + JsonConvert.SerializeObject(settings, Formatting.Indented));

        var mappings = await restEaseApiClient.GetMappingsAsync();
        Console.WriteLine("mappings = " + JsonConvert.SerializeObject(mappings, Formatting.Indented));

        var client = container.CreateClient();
        var result = await client.GetStringAsync("/static/mapping");
        Console.WriteLine("result = " + result);

        await container.StopAsync();
    }
}