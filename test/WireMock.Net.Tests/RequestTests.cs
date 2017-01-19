using System;
using System.Collections.Generic;
using System.Text;
using NFluent;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.Matchers;

namespace WireMock.Net.Tests
{
    [TestFixture]
    public class RequestTests
    {
        [Test]
        public void Should_specify_requests_matching_given_url()
        {
            // given
            var spec = Request.WithUrl("/foo");

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "blabla", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_prefix()
        {
            // given
            var spec = Request.WithUrl("/foo*");

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo/bar"), "blabla", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_url()
        {
            // given
            var spec = Request.WithUrl("/foo");

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/bar"), "blabla", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_path()
        {
            // given
            var spec = Request.WithPath("/foo");

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "blabla", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_put()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingPut();

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_post()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingPost();

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "POST", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_get()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingGet();

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "GET", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_delete()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingDelete();

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "Delete", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_head()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingHead();

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "HEAD", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_matching_given_url_but_not_http_method()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingPut();

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "HEAD", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_exclude_requests_matching_given_http_method_but_not_url()
        {
            // given
            var spec = Request.WithUrl("/bar").UsingPut();

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_headers()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tata");

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, new Dictionary<string, string> { { "X-toto", "tata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_headers()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tatata");

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, new Dictionary<string, string> { { "X-toto", "tata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_headers_ignorecase()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "abc", false);

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, new Dictionary<string, string> { { "X-toto", "ABC" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_header_prefix()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tata*");

            // when
            string bodyAsString = "whatever";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, new Dictionary<string, string> { { "X-toto", "TaTaTa" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody("Hello world!");

            // when
            string bodyAsString = "Hello world!";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body_as_regex()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody("H.*o");

            // when
            string bodyAsString = "Hello world!";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body_as_regexmatcher()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody(new RegexMatcher("H.*o"));

            // when
            string bodyAsString = "Hello world!";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body_as_xpathmatcher_true()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody(new XPathMatcher("/todo-list[count(todo-item) = 3]"));

            // when
            string xmlBodyAsString = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                        <todo-item id='a2'>def</todo-item>
                        <todo-item id='a3'>xyz</todo-item>
                    </todo-list>";
            byte[] body = Encoding.UTF8.GetBytes(xmlBodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, xmlBodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body_as_xpathmatcher_false()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody(new XPathMatcher("/todo-list[count(todo-item) = 99]"));

            // when
            string xmlBodyAsString = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                        <todo-item id='a2'>def</todo-item>
                        <todo-item id='a3'>xyz</todo-item>
                    </todo-list>";
            byte[] body = Encoding.UTF8.GetBytes(xmlBodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, xmlBodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_body_as_jsonpathmatcher_true()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

            // when
            string bodyAsString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body_as_jsonpathmatcher_false()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

            // when
            string bodyAsString = "{ \"things\": { \"name\": \"Wiremock\" } }";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_body()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody("      Hello world!   ");

            // when
            string bodyAsString = "xxx";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", body, bodyAsString, new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_params()
        {
            // given
            var spec = Request.WithPath("/foo").WithParam("bar", "1", "2");

            // when
            string bodyAsString = "Hello world!";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo?bar=1&bar=2"), "PUT", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_params_func()
        {
            // given
            var spec = Request.WithPath("/foo").WithParam(p => p.ContainsKey("bar") && (p["bar"].Contains("1") || p["bar"].Contains("2")));

            // when
            string bodyAsString = "Hello world!";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/foo?bar=1&bar=2"), "PUT", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_params()
        {
            // given
            var spec = Request.WithPath("/foo").WithParam("bar", "1");

            // when
            string bodyAsString = "Hello world!";
            byte[] body = Encoding.UTF8.GetBytes(bodyAsString);
            var request = new RequestMessage(new Uri("http://localhost/test=7"), "PUT", body, bodyAsString);

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }
    }
}

