using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using NFluent;
using NUnit.Framework;
using WireMock.Http;

[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules", 
        "SA1600:ElementsMustBeDocumented", 
        Justification = "Reviewed. Suppression is OK here, as it's a tests class.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules", 
        "SA1633:FileMustHaveHeader", 
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]

namespace WireMock.Net.Tests.Http
{
    [TestFixture]
    public class TinyHttpServerTests
    {
        [Test]
        public void Should_Call_Handler_on_Request()
        {
            // given
            var port = Ports.FindFreeTcpPort();
            bool called = false;
            var urlPrefix = "http://localhost:" + port + "/";
            var server = new TinyHttpServer(urlPrefix, ctx => called = true);
            server.Start();

            // when
            var httpClient = new HttpClient();
            httpClient.GetAsync(urlPrefix).Wait(3000);

            // then
            Check.That(called).IsTrue();
        }
    }
}
