using WireMock.Server;

namespace WireMock.Net.NUnitTests;

[TestFixture]
public class Test
{
    private WireMockServer server;

    [SetUp]
    public void Setup()
    {
        server = WireMockServer.Start();
    }

    [Test]
    public void SomeTest()
    {
        Assert.Pass();
    }

    [TearDown]
    public void TearDown()
    {
        server.Stop();
    }
}