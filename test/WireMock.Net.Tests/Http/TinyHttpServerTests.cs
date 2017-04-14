using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using NFluent;
using Xunit;
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
    //[TestFixture]
    public class TinyHttpServerTests
    {
        [Fact]
        public void Should_call_handler_on_request()
        {
            // given
            var port = PortUtil.FindFreeTcpPort();
            bool called = false;
            var urlPrefix = "http://localhost:" + port + "/";
            var server = new TinyHttpServer((ctx, token) => called = true, urlPrefix);
            server.Start();

            // when
            var httpClient = new HttpClient();
            httpClient.GetAsync(urlPrefix).Wait(3000);

            // then
            Check.That(called).IsTrue();
        }
    }
}
