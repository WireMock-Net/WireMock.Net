using System;
using System.Text;
using NFluent;
using NUnit.Framework;

namespace WireMock.Net.Tests
{
    [TestFixture]
    public class RequestMessageTests
    {
        [Test]
        public void Should_handle_empty_query()
        {
            // given
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST");

            // then
            Check.That(request.GetParameter("not_there")).IsNull();
        }

        [Test]
        public void Should_parse_query_params()
        {
            // given
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost?foo=bar&multi=1&multi=2"), "POST", body, bodyAsString, Encoding.UTF8);

            // then
            Check.That(request.GetParameter("foo")).Contains("bar");
            Check.That(request.GetParameter("multi")).Contains("1");
            Check.That(request.GetParameter("multi")).Contains("2");
        }
    }
}
