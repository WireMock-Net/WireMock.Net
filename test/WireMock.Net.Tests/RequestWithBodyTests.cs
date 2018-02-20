using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests
{
    public class RequestWithBodyTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public void Request_WithBodyExactMatcher()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new ExactMatcher("cat"));

            // when
            string bodyAsString = "cat";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyExactMatcher_multiplePatterns()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new ExactMatcher("cat", "dog"));

            // when
            string bodyAsString = "cat";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(0.5);
        }

        [Fact]
        public void Request_WithBodyExactMatcher_false()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new ExactMatcher("cat"));

            // when
            string bodyAsString = "caR";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsStrictlyLessThan(1.0);
        }

        [Fact]
        public void Request_WithBodySimMetricsMatcher1()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new SimMetricsMatcher("The cat walks in the street."));

            // when
            string bodyAsString = "The car drives in the street.";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsStrictlyLessThan(1.0).And.IsStrictlyGreaterThan(0.5);
        }

        [Fact]
        public void Request_WithBodySimMetricsMatcher2()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyVerb().WithBody(new SimMetricsMatcher("The cat walks in the street."));

            // when
            string bodyAsString = "Hello";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsStrictlyLessThan(0.1).And.IsStrictlyGreaterThan(0.05);
        }

        [Fact]
        public void Request_WithBodyWildcardMatcher()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingAnyVerb().WithBody(new WildcardMatcher("H*o*"));

            // when
            string bodyAsString = "Hello world!";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body, bodyAsString, Encoding.UTF8, new Dictionary<string, string[]> { { "X-toto", new[] { "tatata" } } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyRegexMatcher()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new RegexMatcher("H.*o"));

            // when
            string bodyAsString = "Hello world!";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyXPathMatcher_true()
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
            byte[] body = Encoding.UTF8.GetBytes(xmlBodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body, xmlBodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyXPathMatcher_false()
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
            byte[] body = Encoding.UTF8.GetBytes(xmlBodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body, xmlBodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyJsonPathMatcher_true()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

            // when
            string bodyAsString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyJsonPathMatcher_false()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

            // when
            string bodyAsString = "{ \"things\": { \"name\": \"Wiremock\" } }";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body, bodyAsString, Encoding.UTF8);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyAsJson_JsonPathMatcher_true()
        {
            // given
            var spec = Request.Create().UsingAnyVerb().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

            // when
            string jsonString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                Encoding = Encoding.UTF8
            };

            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, bodyData);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }
    }
}