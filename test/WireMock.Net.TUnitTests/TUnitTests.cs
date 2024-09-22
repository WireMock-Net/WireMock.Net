// Copyright © WireMock.Net

using WireMock.Net.TUnit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.TUnitTests;

// ReSharper disable once InconsistentNaming
public class TUnitTests
{
    [Test]
    public async Task Test_TUnitWireMockLogger()
    {
        // Assign
        var path = $"/foo_{Guid.NewGuid()}";

        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TUnitWireMockLogger(TestContext.Current!.GetDefaultLogger())
        });

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create().WithBody("TUnit"));

        // Act
        var response = await server.CreateClient().GetStringAsync($"{server.Url}{path}");

        // Assert
        await Assert.That(response).IsEqualTo("TUnit");
    }
}