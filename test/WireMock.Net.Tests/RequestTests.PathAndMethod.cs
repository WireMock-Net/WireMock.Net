using System;
using System.Text;
using NFluent;
using Xunit;
using WireMock.RequestBuilders;
using WireMock.Matchers.Request;

namespace WireMock.Net.Tests
{
    //[TestFixture]
    public partial class RequestTests
    {
        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_delete()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingDelete();

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "Delete", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_get()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingGet();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "GET", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_head()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingHead();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "HEAD", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_post()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingPost();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_put()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingPut();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_patch()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingPatch();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PATCH", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_exclude_requests_matching_given_path_but_not_http_method()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingPut();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "HEAD", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }
    }
}