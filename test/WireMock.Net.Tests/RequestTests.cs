using System;
using System.Collections.Generic;
using System.Text;
using NFluent;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.Net.Tests
{
    [TestFixture]
    public class RequestTests
    {
        [Test]
        public void Should_specify_requests_matching_given_path()
        {
            // given
            var spec = Request.Create().WithPath("/foo");

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "blabla");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_paths()
        {
            var requestBuilder = Request.Create().WithPath("/x1", "/x2");

            var request1 = new RequestMessage(new Uri("http://localhost/x1"), "blabla");
            var request2 = new RequestMessage(new Uri("http://localhost/x2"), "blabla");

            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request1, requestMatchResult)).IsEqualTo(1.0);
            Check.That(requestBuilder.GetMatchingScore(request2, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_pathFuncs()
        {
            // given
            var spec = Request.Create().WithPath(url => url.EndsWith("/foo"));

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "blabla");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_path_prefix()
        {
            // given
            var spec = Request.Create().WithPath(new RegexMatcher("^/foo"));

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo/bar"), "blabla");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_path()
        {
            // given
            var spec = Request.Create().WithPath("/foo");

            // when
            var request = new RequestMessage(new Uri("http://localhost/bar"), "blabla");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_url()
        {
            // given
            var spec = Request.Create().WithUrl("*/foo");

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "blabla");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_path_and_method_put()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingPut();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_path_and_method_post()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingPost();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_path_and_method_get()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingGet();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "GET");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_path_and_method_delete()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingDelete();

            // when
            var bodyAsString = "whatever";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "Delete", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_path_and_method_head()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingHead();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "HEAD");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_exclude_requests_matching_given_path_but_not_http_method()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingPut();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "HEAD");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Test]
        public void Should_exclude_requests_matching_given_http_method_but_not_url()
        {
            // given
            var spec = Request.Create().WithPath("/bar").UsingPut();

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_path_and_headers()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingAnyVerb().WithHeader("X-toto", "tata");

            // when
            var bodyAsString = "whatever";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8, new Dictionary <string, string> { { "X-toto", "tata" } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_headers()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithHeader("X-toto", "tatata");

            // when
            var bodyAsString = "whatever";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8, new Dictionary <string, string> { { "X-toto", "tata" } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_headers_ignorecase()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithHeader("X-toto", "abc", false);

            // when
            var bodyAsString = "whatever";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8, new Dictionary <string, string> { { "X-toto", "ABC" } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_header_prefix()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithHeader("X-toto", "tata*");

            // when
            var bodyAsString = "whatever";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8, new Dictionary<string, string> { { "X-toto", "TaTa" } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_cookies()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithCookie("session", "a*");

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", null, null, null, null, new Dictionary<string, string> { { "session", "abc" } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody("Hello world!");

            // when
            var bodyAsString = "Hello world!";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_ExactMatcher_true()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new ExactMatcher("cat"));

            // when
            var bodyAsString = "cat";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_ExactMatcher_multiplePatterns()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new ExactMatcher("cat", "dog"));

            // when
            var bodyAsString = "cat";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(0.5);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_ExactMatcher_false()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new ExactMatcher("cat"));

            // when
            var bodyAsString = "caR";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsLessThan(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_SimMetricsMatcher1()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new SimMetricsMatcher("The cat walks in the street."));

            // when
            var bodyAsString = "The car drives in the street.";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsLessThan(1.0).And.IsGreaterThan(0.5);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_SimMetricsMatcher2()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new SimMetricsMatcher("The cat walks in the street."));

            // when
            var bodyAsString = "Hello";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsLessThan(0.1).And.IsGreaterThan(0.05);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_WildcardMatcher()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingAnyVerb().WithBody(new WildcardMatcher("H*o*"));

            // when
            var bodyAsString = "Hello world!";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8, new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_RegexMatcher()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new RegexMatcher("H.*o"));

            // when
            var bodyAsString = "Hello world!";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_XPathMatcher_true()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new XPathMatcher("/todo-list[count(todo-item) = 3]"));

            // when
            string xmlBodyAsString = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                        <todo-item id='a2'>def</todo-item>
                        <todo-item id='a3'>xyz</todo-item>
                    </todo-list>";
            var body = Encoding.UTF8.GetBytes(xmlBodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, xmlBodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_XPathMatcher_false()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new XPathMatcher("/todo-list[count(todo-item) = 99]"));

            // when
            string xmlBodyAsString = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                        <todo-item id='a2'>def</todo-item>
                        <todo-item id='a3'>xyz</todo-item>
                    </todo-list>";
            var body = Encoding.UTF8.GetBytes(xmlBodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, xmlBodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_JsonPathMatcher_true()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

            // when
            var bodyAsString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_body_using_JsonPathMatcher_false()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

            // when
            var bodyAsString = "{ \"things\": { \"name\": \"Wiremock\" } }";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_body()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody("      Hello world!   ");

            // when
            var bodyAsString = "xxx";
            var body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, Encoding.UTF8, new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_param()
        {
            // given
            var spec = Request.Create().WithParam("bar", "1", "2");

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo?bar=1&bar=2"), "PUT");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_paramNoValue()
        {
            // given
            var spec = Request.Create().WithParam("bar");

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo?bar"), "PUT");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_specify_requests_matching_given_param_func()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithParam(p => p.ContainsKey("bar"));

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo?bar=1&bar=2"), "PUT");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_params()
        {
            // given
            var spec = Request.Create().WithParam("bar", "1");

            // when
            var request = new RequestMessage(new Uri("http://localhost/test=7"), "PUT");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }
    }
}