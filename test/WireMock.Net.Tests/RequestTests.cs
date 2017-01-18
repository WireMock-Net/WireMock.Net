using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NFluent;
using NUnit.Framework;
using WireMock.RequestBuilders;

[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK here, as it's a tests class.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
// ReSharper disable InconsistentNaming
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
            var request = new RequestMessage("/foo", string.Empty, "blabla", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_prefix()
        {
            // given
            var spec = Request.WithUrl("/foo*");

            // when
            var request = new RequestMessage("/foo/bar", string.Empty, "blabla", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_url()
        {
            // given
            var spec = Request.WithUrl("/foo");

            // when
            var request = new RequestMessage("/bar", string.Empty, "blabla", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_path()
        {
            // given
            var spec = Request.WithPath("/foo");

            // when
            var request = new RequestMessage("/foo", "?param=1", "blabla", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_put()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingPut();

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_post()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingPost();

            // when
            var request = new RequestMessage("/foo", string.Empty, "POST", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_get()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingGet();

            // when
            var request = new RequestMessage("/foo", string.Empty, "GET", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_delete()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingDelete();

            // when
            var request = new RequestMessage("/foo", string.Empty, "DELETE", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method_head()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingHead();

            // when
            var request = new RequestMessage("/foo", string.Empty, "HEAD", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_matching_given_url_but_not_http_method()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingPut();

            // when
            var request = new RequestMessage("/foo", string.Empty, "POST", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_exclude_requests_matching_given_http_method_but_not_url()
        {
            // given
            var spec = Request.WithUrl("/bar").UsingPut();

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_headers()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tata");

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string> { { "X-toto", "tata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_headers()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tatata");

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string> { { "X-toto", "tata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_headers_ignorecase()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "abc", false);

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string> { { "X-toto", "ABC" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_header_prefix()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tata*");

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string> { { "X-toto", "TaTaTa" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody(".*Hello world!.*");

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "Hello world!", new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body_as_wildcard()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody("H.*o");

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "Hello world!", new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_body()
        {
            // given
            var spec = Request.WithUrl("/foo").UsingAnyVerb().WithBody("      Hello world!   ");

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "XXXXXXXXXXX", new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_params()
        {
            // given
            var spec = Request.WithPath("/foo").WithParam("bar", "1", "2");

            // when
            var request = new RequestMessage("/foo", "bar=1&bar=2", "Get", "Hello world!", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_params_func()
        {
            // given
            var spec = Request.WithPath("/foo").WithParam(p => p.ContainsKey("bar") && (p["bar"].Contains("1") || p["bar"].Contains("2")));

            // when
            var request = new RequestMessage("/foo", "bar=1&bar=2", "Get", "Hello world!", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_params()
        {
            // given
            var spec = Request.WithPath("/foo").WithParam("bar", "1");

            // when
            var request = new RequestMessage("/foo", string.Empty, "PUT", "XXXXXXXXXXX", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }
    }
}

