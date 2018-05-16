using System;
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
        public void Request_WithBody_FuncString()
        {
            // Assign
            var requestBuilder = Request.Create().UsingAnyMethod().WithBody(b => b.Contains("b"));

            // Act
            var body = new BodyData
            {
                BodyAsString = "b"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBody_FuncJson()
        {
            // Assign
            var requestBuilder = Request.Create().UsingAnyMethod().WithBody(b => b != null);

            // Act
            var body = new BodyData
            {
                BodyAsJson = 123
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBody_FuncByteArray()
        {
            // Assign
            var requestBuilder = Request.Create().UsingAnyMethod().WithBody((byte[] b) => b != null);

            // Act
            var body = new BodyData
            {
                BodyAsBytes = new byte[0]
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyExactMatcher()
        {
            // given
            var requestBuilder = Request.Create().UsingAnyMethod().WithBody(new ExactMatcher("cat"));

            // when
            var body = new BodyData
            {
                BodyAsString = "cat"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", ClientIp, body);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyWildcardMatcher()
        {
            // given
            var spec = Request.Create().WithPath("/foo").UsingAnyMethod().WithBody(new WildcardMatcher("H*o*"));

            // when
            var body = new BodyData
            {
                BodyAsString = "Hello world!"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyXPathMatcher_true()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody(new XPathMatcher("/todo-list[count(todo-item) = 3]"));

            // when
            var body = new BodyData
            {
                BodyAsString = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                        <todo-item id='a2'>def</todo-item>
                        <todo-item id='a3'>xyz</todo-item>
                    </todo-list>"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyXPathMatcher_false()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody(new XPathMatcher("/todo-list[count(todo-item) = 99]"));

            // when
            var body = new BodyData
            {
                BodyAsString = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                        <todo-item id='a2'>def</todo-item>
                        <todo-item id='a3'>xyz</todo-item>
                    </todo-list>"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyJsonPathMatcher_true()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$..things[?(@.name == 'RequiredThing')]"));

            // when
            var body = new BodyData
            {
                BodyAsString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyJsonPathMatcher_false()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

            // when
            var body = new BodyData
            {
                BodyAsString = "{ \"things\": { \"name\": \"Wiremock\" } }"
            };
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, body);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyAsJson_Object_JsonPathMatcher_true()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$..things[?(@.name == 'RequiredThing')]"));

            // when
            string jsonString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                BodyAsString = jsonString,
                Encoding = Encoding.UTF8
            };

            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, bodyData);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyAsJson_Array_JsonPathMatcher_1()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$..books[?(@.price < 10)]"));

            // when
            string jsonString = "{ \"books\": [ { \"category\": \"test1\", \"price\": 8.95 }, { \"category\": \"test2\", \"price\": 20 } ] }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                BodyAsString = jsonString,
                Encoding = Encoding.UTF8
            };

            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, bodyData);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyAsJson_Array_JsonPathMatcher_2()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$..[?(@.Id == 1)]"));

            // when
            string jsonString = "{ \"Id\": 1, \"Name\": \"Test\" }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                BodyAsString = jsonString,
                Encoding = Encoding.UTF8
            };

            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, bodyData);

            // then
            var requestMatchResult = new RequestMatchResult();
            double result = spec.GetMatchingScore(request, requestMatchResult);
            Check.That(result).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyAsObject_ExactObjectMatcher_true()
        {
            // Assign
            object body = DateTime.MinValue;
            var requestBuilder = Request.Create().UsingAnyMethod().WithBody(body);

            var bodyData = new BodyData
            {
                BodyAsJson = DateTime.MinValue
            };

            // Act
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, bodyData);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithBodyAsBytes_ExactObjectMatcher_true()
        {
            // Assign
            byte[] body = { 123 };
            var requestBuilder = Request.Create().UsingAnyMethod().WithBody(body);

            var bodyData = new BodyData
            {
                BodyAsBytes = new byte[] { 123 }
            };

            // Act
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, bodyData);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }
    }
}