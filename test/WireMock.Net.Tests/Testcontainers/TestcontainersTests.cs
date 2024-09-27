// Copyright © WireMock.Net

#if NET6_0_OR_GREATER
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock.Net.Testcontainers;
using Xunit;

namespace WireMock.Net.Tests.Testcontainers;

public class TestcontainersTests
{
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