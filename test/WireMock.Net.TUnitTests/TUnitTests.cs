// Copyright © WireMock.Net

using WireMock.Logging;
using WireMock.Net.TUnit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.TUnitTests;

// ReSharper disable once InconsistentNaming
public class TUnitTests
{
    private IWireMockLogger _logger = null!;

    [Before(Test)]
    public void BeforeTest(TestContext context)
    {
        _logger = new TUnitWireMockLogger(context.GetDefaultLogger());
    }

    [Test]
    public async Task Test_TUnitWireMockLogger()
    {
        // Assign
        var path = $"/foo_{Guid.NewGuid()}";

        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = _logger
        });

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create().WithBody("TUnit"));

        // Act
        var response = await new HttpClient().GetStringAsync($"{server.Url}{path}").ConfigureAwait(false);

        // Assert
        await Assert.That(response).IsEqualTo("TUnit");
    }
}