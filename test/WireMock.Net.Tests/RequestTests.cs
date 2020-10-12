using System.Collections.Generic;
using FluentAssertions;
using NFluent;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests
{
    public class RequestTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public void Should_Match_When_Verb_Does_Match()
        {
            // Arrange
            var requestPut = Request.Create().WithPath("/bar").UsingPut();

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/bar"), "PUT", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            requestPut.GetMatchingScore(request, requestMatchResult).Should().Be(1.0);
        }

        [Fact]
        public void Should_NotMatch_When_Verb_Does_Not_Match()
        {
            // Arrange
            var requestGet = Request.Create().WithPath("/bar").UsingGet();

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/bar"), "PUT", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            requestGet.GetMatchingScore(request, requestMatchResult).Should().Be(0.0);
        }

        [Fact]
        public void Should_exclude_requests_matching_given_http_method_but_not_url()
        {
            // given
            var spec = Request.Create().WithPath("/bar").UsingPut();

            // when
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Should_exclude_requests_not_matching_given_headers()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithHeader("X-toto", "tatata");

            // when
            var body = new BodyData
            {
                BodyAsString = "whatever",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, new Dictionary<string, string[]> { { "X-toto", new[] { "tata" } } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Should_exclude_requests_not_matching_given_headers_ignorecase()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithHeader("X-toto", "abc", false);

            // when
            var body = new BodyData
            {
                BodyAsString = "whatever",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, new Dictionary<string, string[]> { { "X-toto", new[] { "ABC" } } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_header_prefix()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithHeader("X-toto", "tata*");

            // when
            var body = new BodyData
            {
                BodyAsString = "whatever",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, new Dictionary<string, string[]> { { "X-toto", new[] { "TaTa" } } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_body()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody("Hello world!");

            // when
            var body = new BodyData
            {
                BodyAsString = "Hello world!",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_exclude_requests_not_matching_given_body()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody("      Hello world!   ");

            // when
            var body = new BodyData
            {
                BodyAsString = "xxx",
                DetectedBodyType = BodyType.String
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, new Dictionary<string, string[]> { { "X-toto", new[] { "tata" } } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_param()
        {
            // given
            var spec = Request.Create().WithParam("bar", "1", "2");

            // when
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?bar=1&bar=2"), "PUT", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_param_func()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithParam(p => p.ContainsKey("bar"));

            // when
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?bar=1&bar=2"), "PUT", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_exclude_requests_not_matching_given_params()
        {
            // given
            var spec = Request.Create().WithParam("bar", "1");

            // when
            var request = new RequestMessage(new UrlDetails("http://localhost/test=7"), "PUT", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }
    }
}