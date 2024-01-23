#if NET6_0_OR_GREATER
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock.Net.Testcontainers;
using Xunit;

namespace WireMock.Net.Tests.Testcontainers;

public class TestcontainersTests
{
    [Fact]
    public async Task WireMockContainer_Build_and_StartAsync_and_StopAsync()
    {
        // Act
        var wireMockContainer = new WireMockContainerBuilder()
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

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