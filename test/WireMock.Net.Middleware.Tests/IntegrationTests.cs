// Copyright © WireMock.Net

using FluentAssertions;
using WireMock.Net.TestWebApplication;

namespace WireMock.Net.Middleware.Tests;

public class IntegrationTests
{
    [Theory]
    [InlineData("/real1", "Hello 1 from WireMock.Net !")]
    [InlineData("/real2", "Hello 2 from WireMock.Net !")]
    public async Task CallingRealApi_WithAlwaysRedirectToWireMockIsTrue(string requestUri, string expectedResponse)
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(requestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        stringResponse.Should().Be(expectedResponse);
    }

    [Theory]
    [InlineData("/real1", "Hello 1 from WireMock.Net !")]
    [InlineData("/real2", "Hello 2 from WireMock.Net !")]
    public async Task CallingRealApi_WithAlwaysRedirectToWireMockIsFalse(string requestUri, string expectedResponse)
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory<Program>(false);
        using var client = factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("X-WireMock-Redirect", "true");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        stringResponse.Should().Be(expectedResponse);
    }
}