using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xamarin.UITest;

namespace WireMock.Net.XamarinUI.Tests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class UIFlowTests
    {
        IApp app;
        Platform platform;

        private WireMockServer _server;

        public UIFlowTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            var serverSettings = new WireMockServerSettings();
            // serverSettings.Port = 5005;
            serverSettings.AllowPartialMapping = true;

            _server = WireMockServer.Start(serverSettings);
        }

        [Test]
        public async Task TestMockServerIsWorking()
        {
            _server
                .Given(Request.Create()
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody("test"));

            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync(_server.Urls[0]);
            var content = await result.Content.ReadAsStringAsync();
            Assert.AreEqual("test", content);
        }

        [TearDown]
        public void ShutdownServer()
        {
            _server.Stop();
        }
    }
}