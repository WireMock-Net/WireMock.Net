using System.Collections.Generic;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests
{
    public class RequestWithPathTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public void Request_WithPath_Spaces()
        {
            // Assign
            var spec = Request.Create().WithPath("/path/a b").UsingAnyMethod();

            // Act
            var body = new BodyData();
            var request = new RequestMessage(new UrlDetails("http://localhost/path/a b"), "GET", ClientIp, body);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithPath_WithHeader_Match()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo").UsingAnyMethod().WithHeader("X-toto", "tata");

            // Act
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, new Dictionary<string, string[]> { { "X-toto", new[] { "tata" } } });

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithPath()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo");

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "blabla", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithPaths()
        {
            var requestBuilder = Request.Create().WithPath("/x1", "/x2");

            var request1 = new RequestMessage(new UrlDetails("http://localhost/x1"), "blabla", ClientIp);
            var request2 = new RequestMessage(new UrlDetails("http://localhost/x2"), "blabla", ClientIp);

            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request1, requestMatchResult)).IsEqualTo(1.0);
            Check.That(requestBuilder.GetMatchingScore(request2, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithPathFunc()
        {
            // Arrange
            var spec = Request.Create().WithPath(url => url.EndsWith("/foo"));

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "blabla", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithPathRegexMatcher_HasMatch()
        {
            // Arrange
            var spec = Request.Create().WithPath(new RegexMatcher("^/foo"));

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo/bar"), "blabla", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithPathRegexMatcher_HasNoMatch()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo");

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/bar"), "blabla", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Request_WithPathRegexMatcher_WithPatternAsFile_HasMatch()
        {
            // Arrange
            var pattern = new StringPattern
            {
                Pattern = "^/foo",
                PatternAsFile = "c:\\x.txt"
            };
            var spec = Request.Create().WithPath(new RegexMatcher(pattern));

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo/bar"), "blabla", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_delete()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo").UsingDelete();

            // Act
            var body = new BodyData
            {
                BodyAsString = "whatever"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "Delete", ClientIp, body);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_get()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo").UsingGet();

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_head()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo").UsingHead();

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "HEAD", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_post()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo").UsingPost();

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_put()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo").UsingPut();

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_specify_requests_matching_given_path_and_method_patch()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo").UsingPatch();

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PATCH", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Should_exclude_requests_matching_given_path_but_not_http_method()
        {
            // Arrange
            var spec = Request.Create().WithPath("/foo").UsingPut();

            // Act
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "HEAD", ClientIp);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }
    }
}