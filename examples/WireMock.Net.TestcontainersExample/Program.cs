// Copyright © WireMock.Net

using DotNet.Testcontainers.Configurations;
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
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Copy");
            await TestCopyAsync();
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
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Automatic");
            await TestAsync();
        }
        finally
        {
            Console.ForegroundColor = original;
        }

        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Linux");
            await TestAsync("sheyenrath/wiremock.net:1.6.5");
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
            await TestAsync("sheyenrath/wiremock.net-alpine:1.6.5");
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
            await TestAsync("sheyenrath/wiremock.net-windows:1.6.5");
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
    }

    private static async Task TestCopyAsync()
    {
        var builder = new WireMockContainerBuilder()
            .WithWatchStaticMappings(true)
            .WithAutoRemove(true)
            .WithCleanUp(true);

        var container = builder.Build();

        await container.StartAsync();

        if (await GetImageOSAsync.Value == OSPlatform.Linux)
        {
            try
            {
                await container.CopyAsync(@"C:\temp-wiremock\__admin\mappings\StefBodyAsFileExample.json", "/app/__admin/mappings");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        Console.WriteLine("PublicUrl = " + container.GetPublicUrl());

        var adminClient = container.CreateWireMockAdminClient();

        var mappings = await adminClient.GetMappingsAsync();
        Console.WriteLine("mappings = " + JsonConvert.SerializeObject(mappings, Formatting.Indented));

        await Task.Delay(1_000);

        //Console.WriteLine("Press any key to stop.");
        //Console.ReadKey();

        await container.StopAsync();
    }

    private static async Task TestAsync(string? image = null)
    {
        var mappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "WireMock.Net.Console.NET6", "__admin", "mappings");

        var builder = new WireMockContainerBuilder()
            .WithAdminUserNameAndPassword("x", "y")
            .WithMappings(mappingsPath)
            .WithWatchStaticMappings(true)
            .WithAutoRemove(true)
            .WithCleanUp(true);

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

        await container.ReloadStaticMappingsAsync();

        //var logs = await container.GetLogsAsync(DateTime.Now.AddDays(-1));
        //Console.WriteLine("logs = " + logs.Stdout);

        Console.WriteLine("PublicUrl = " + container.GetPublicUrl());

        var restEaseApiClient = container.CreateWireMockAdminClient();

        var settings = await restEaseApiClient.GetSettingsAsync();
        Console.WriteLine("settings = " + JsonConvert.SerializeObject(settings, Formatting.Indented));

        var mappings = await restEaseApiClient.GetMappingsAsync();
        Console.WriteLine("mappingsStef = " + JsonConvert.SerializeObject(mappings, Formatting.Indented));

        var client = container.CreateClient();
        var result = await client.GetStringAsync("/static/mapping");
        Console.WriteLine("result = " + result);

        //if (image == null)
        //{
        //    Console.WriteLine("Press any key to stop.");
        //    Console.ReadKey();
        //}

        await container.StopAsync();
    }

    private static Lazy<Task<OSPlatform>> GetImageOSAsync = new(async () =>
    {
        if (TestcontainersSettings.OS.DockerEndpointAuthConfig == null)
        {
            throw new InvalidOperationException($"The {nameof(TestcontainersSettings.OS.DockerEndpointAuthConfig)} is null. Check if Docker is started.");
        }

        using var dockerClientConfig = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration();
        using var dockerClient = dockerClientConfig.CreateClient();

        var version = await dockerClient.System.GetVersionAsync();
        return version.Os.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) >= 0 ? OSPlatform.Windows : OSPlatform.Linux;
    });
}