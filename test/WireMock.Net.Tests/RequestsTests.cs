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
    public class RequestsTests
    {
        [Test]
        public void Should_specify_requests_matching_given_url()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo");

            // when
            var request = new Request("/foo", string.Empty, "blabla", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_prefix()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo*");

            // when
            var request = new Request("/foo/bar", string.Empty, "blabla", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_url()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo");

            // when
            var request = new Request("/bar", string.Empty, "blabla", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_path()
        {
            // given
            var spec = RequestBuilder.WithPath("/foo");

            // when
            var request = new Request("/foo", "?param=1", "blabla", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_method()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingPut();

            // when
            var request = new Request("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_matching_given_url_but_not_http_method()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingPut();

            // when
            var request = new Request("/foo", string.Empty, "POST", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_exclude_requests_matching_given_http_method_but_not_url()
        {
            // given
            var spec = RequestBuilder.WithUrl("/bar").UsingPut();

            // when
            var request = new Request("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_url_and_headers()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tata");

            // when
            var request = new Request("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string> { { "X-toto", "tata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_headers()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tatata");

            // when
            var request = new Request("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string> { { "X-toto", "tata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_headers_ignorecase()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "abc", false);

            // when
            var request = new Request("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string> { { "X-toto", "ABC" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_header_prefix()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingAnyVerb().WithHeader("X-toto", "tata*");

            // when
            var request = new Request("/foo", string.Empty, "PUT", "whatever", new Dictionary<string, string> { { "X-toto", "TaTaTa" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingAnyVerb().WithBody(".*Hello world!.*");

            // when
            var request = new Request("/foo", string.Empty, "PUT", "Hello world!", new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_specify_requests_matching_given_body_as_wildcard()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingAnyVerb().WithBody("H.*o");

            // when
            var request = new Request("/foo", string.Empty, "PUT", "Hello world!", new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_body()
        {
            // given
            var spec = RequestBuilder.WithUrl("/foo").UsingAnyVerb().WithBody("      Hello world!   ");

            // when
            var request = new Request("/foo", string.Empty, "PUT", "XXXXXXXXXXX", new Dictionary<string, string> { { "X-toto", "tatata" } });

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }

        [Test]
        public void Should_specify_requests_matching_given_params()
        {
            // given
            var spec = RequestBuilder.WithPath("/foo").WithParam("bar", "1", "2");

            // when
            var request = new Request("/foo", "bar=1&bar=2", "Get", "Hello world!", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsTrue();
        }

        [Test]
        public void Should_exclude_requests_not_matching_given_params()
        {
            // given
            var spec = RequestBuilder.WithPath("/foo").WithParam("bar", "1");

            // when
            var request = new Request("/foo", string.Empty, "PUT", "XXXXXXXXXXX", new Dictionary<string, string>());

            // then
            Check.That(spec.IsSatisfiedBy(request)).IsFalse();
        }
    }
}
