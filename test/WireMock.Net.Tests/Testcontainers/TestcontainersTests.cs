// Copyright © WireMock.Net

#if NET6_0_OR_GREATER
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock.Net.Testcontainers;
using WireMock.Net.Tests.Facts;
using Xunit;

namespace WireMock.Net.Tests.Testcontainers;

public class TestcontainersTests
{
    [Fact]
    public async Task WireMockContainer_Build_WithNoImage_TestGrpc1()
    {
        // Act
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var wireMockContainer = new WireMockContainerBuilder()
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .WithAdminUserNameAndPassword(adminUsername, adminPassword)
            .WithCommand("--UseHttp2")
            .WithEntrypoint("./wiremock-net", "--Urls", "http://*:80", "grpc://*:9090")
            .WithPortBinding(9090, true)
            .Build();

        try
        {
            await wireMockContainer.StartAsync().ConfigureAwait(false);

            // Assert
            using (new AssertionScope())
            {
                var logs = await wireMockContainer.GetLogsAsync(DateTime.MinValue);
                logs.Should().NotBeNull();

                var url = wireMockContainer.GetPublicUrl();
                url.Should().NotBeNullOrWhiteSpace();

                var urls = wireMockContainer.GetPublicUrls();
                urls.Should().BeNullOrEmpty();
                urls.Should().HaveCount(2);

                var httpPort = wireMockContainer.GetMappedPublicPort(80);
                httpPort.Should().BeGreaterThan(0);

                var grpcPort = wireMockContainer.GetMappedPublicPort(9090);
                grpcPort.Should().BeGreaterThan(0);

                var adminClient = wireMockContainer.CreateWireMockAdminClient();

                var settings = await adminClient.GetSettingsAsync();
                settings.Should().NotBeNull();
            }
        }
        finally
        {
            await wireMockContainer.StopAsync();
        }
    }

    [Fact]
    public async Task WireMockContainer_Build_WithNoImage_TestGrpc2()
    {
        // Act
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var wireMockContainer = new WireMockContainerBuilder()
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .WithAdminUserNameAndPassword(adminUsername, adminPassword)
            .AddUrl("http://*:8080")
            .AddUrl("grpc://*:9090")
            .AddUrl("grpc://*:9091")
            .Build();

        try
        {
            await wireMockContainer.StartAsync().ConfigureAwait(false);

            // Assert
            using (new AssertionScope())
            {
                var logs = await wireMockContainer.GetLogsAsync(DateTime.MinValue);
                logs.Should().NotBeNull();

                var url = wireMockContainer.GetPublicUrl();
                url.Should().NotBeNullOrWhiteSpace();

                var urls = wireMockContainer.GetPublicUrls();
                urls.Should().HaveCount(4);
                var urlHttp80 = urls[0];
                urlHttp80.Should().StartWith("http://");

                var urlHttp8080 = urls[1];
                urlHttp8080.Should().StartWith("http://");

                var urlGrpc9090 = urls[2];
                urlGrpc9090.Should().StartWith("grpc://");

                var urlGrpc9091 = urls[3];
                urlGrpc9091.Should().StartWith("grpc://");

                var adminClient = wireMockContainer.CreateWireMockAdminClient();

                var settings = await adminClient.GetSettingsAsync();
                settings.Should().NotBeNull();
            }
        }
        finally
        {
            await wireMockContainer.StopAsync();
        }
    }

    [Fact]
    public async Task WireMockContainer_Build_WithNoImage_And_StartAsync_and_StopAsync()
    {
        // Act
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var wireMockContainer = new WireMockContainerBuilder()
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .WithAdminUserNameAndPassword(adminUsername, adminPassword)
            .Build();

        await StartTestAndStopAsync(wireMockContainer);
    }

    // https://github.com/testcontainers/testcontainers-dotnet/issues/1322
    [RunOnDockerPlatformFact("Linux")]
    public async Task WireMockContainer_Build_WithNoImageAndNetwork_And_StartAsync_and_StopAsync()
    {
        // Act
        var dummyNetwork = new NetworkBuilder()
            .WithName("Dummy Network for TestcontainersTests")
            .WithCleanUp(true)
            .Build();

        var wireMockContainer = new WireMockContainerBuilder()
            .WithNetwork(dummyNetwork)
            .WithWatchStaticMappings(true)
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await StartTestAndStopAsync(wireMockContainer);
    }

    [Fact]
    public async Task WireMockContainer_Build_WithImage_And_StartAsync_and_StopAsync()
    {
        // Arrange
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var wireMockContainerBuilder = new WireMockContainerBuilder()
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .WithAdminUserNameAndPassword(adminUsername, adminPassword);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            wireMockContainerBuilder = wireMockContainerBuilder.WithWindowsImage();
        }
        else
        {
            wireMockContainerBuilder = wireMockContainerBuilder.WithLinuxImage();
        }

        var wireMockContainer = wireMockContainerBuilder.Build();

        await StartTestAndStopAsync(wireMockContainer);
    }

    [Fact]
    public async Task WireMockContainer_Build_WithImageAsText_And_StartAsync_and_StopAsync()
    {
        // Arrange
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var wireMockContainerBuilder = new WireMockContainerBuilder()
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .WithAdminUserNameAndPassword(adminUsername, adminPassword);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            wireMockContainerBuilder = wireMockContainerBuilder.WithImage("sheyenrath/wiremock.net-windows");
        }
        else
        {
            wireMockContainerBuilder = wireMockContainerBuilder.WithImage("sheyenrath/wiremock.net");
        }

        var wireMockContainer = wireMockContainerBuilder.Build();

        await StartTestAndStopAsync(wireMockContainer);
    }

    private static async Task StartTestAndStopAsync(WireMockContainer wireMockContainer)
    {
        try
        {
            await wireMockContainer.StartAsync().ConfigureAwait(false);

            // Assert
            using (new AssertionScope())
            {
                var url = wireMockContainer.GetPublicUrl();
                url.Should().NotBeNullOrWhiteSpace();

                var adminClient = wireMockContainer.CreateWireMockAdminClient();

                var settings = await adminClient.GetSettingsAsync();
                settings.Should().NotBeNull();
            }
        }
        finally
        {
            await wireMockContainer.StopAsync();
        }
    }
}
#endif