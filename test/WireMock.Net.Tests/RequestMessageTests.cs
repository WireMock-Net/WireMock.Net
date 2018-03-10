using System;
using NFluent;
using Xunit;

namespace WireMock.Net.Tests
{
    public class RequestMessageTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public void RequestMessage_ParseQuery_NoKeys()
        {
            // given
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp);

            // then
            Check.That(request.GetParameter("not_there")).IsNull();
        }

        [Fact]
        public void RequestMessage_ParseQuery_SingleKey_SingleValue()
        {
            // Assign
            var request = new RequestMessage(new Uri("http://localhost?foo=bar"), "POST", ClientIp);

            // Assert
            Check.That(request.GetParameter("foo")).ContainsExactly("bar");
        }

        [Fact]
        public void RequestMessage_ParseQuery_MultipleKeys_MultipleValues()
        {
            // Assign
            var request = new RequestMessage(new Uri("http://localhost?key=1&key=2"), "POST", ClientIp);

            // Assert
            Check.That(request.GetParameter("key")).Contains("1");
            Check.That(request.GetParameter("key")).Contains("2");
        }

        [Fact]
        public void RequestMessage_ParseQuery_SingleKey_MultipleValues()
        {
            // Assign
            var request = new RequestMessage(new Uri("http://localhost?key=1,2&foo=bar&key=3"), "POST", ClientIp);

            // Assert
            Check.That(request.GetParameter("key")).Contains("1");
            Check.That(request.GetParameter("key")).Contains("2");
            Check.That(request.GetParameter("key")).Contains("3");
        }
    }
}