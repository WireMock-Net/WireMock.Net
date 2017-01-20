using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Matchers;
using WireMock.Matchers.Request;

[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The requests.
    /// </summary>
    public class Request : RequestMessageCompositeMatcher, IVerbRequestBuilder
    {
        /// <summary>
        /// The _request matchers.
        /// </summary>
        private readonly IList<IRequestMatcher> _requestMatchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="requestMatchers">
        /// The request matchers.
        /// </param>
        private Request(IList<IRequestMatcher> requestMatchers) : base(requestMatchers)
        {
            _requestMatchers = requestMatchers;
        }

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="IVerbRequestBuilder"/>.
        /// </returns>
        public static IVerbRequestBuilder WithUrl(string url)
        {
            var specs = new List<IRequestMatcher> { new RequestMessageUrlMatcher(url) };

            return new Request(specs);
        }

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="func">
        /// The url func.
        /// </param>
        /// <returns>
        /// The <see cref="IVerbRequestBuilder"/>.
        /// </returns>
        public static IVerbRequestBuilder WithUrl(Func<string, bool> func)
        {
            var specs = new List<IRequestMatcher> { new RequestMessageUrlMatcher(func) };

            return new Request(specs);
        }

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="IVerbRequestBuilder"/>.
        /// </returns>
        public static IVerbRequestBuilder WithPath(string path)
        {
            var specs = new List<IRequestMatcher> { new RequestMessagePathMatcher(path) };

            return new Request(specs);
        }

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="func">
        /// The path func.
        /// </param>
        /// <returns>
        /// The <see cref="IVerbRequestBuilder"/>.
        /// </returns>
        public static IVerbRequestBuilder WithPath([NotNull] Func<string, bool> func)
        {
            var specs = new List<IRequestMatcher> { new RequestMessagePathMatcher(func) };

            return new Request(specs);
        }

        /// <summary>
        /// The using get.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingGet()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("get"));
            return this;
        }

        /// <summary>
        /// The using post.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingPost()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("post"));
            return this;
        }

        /// <summary>
        /// The using put.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingPut()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("put"));
            return this;
        }

        /// <summary>
        /// The using delete.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingDelete()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("delete"));
            return this;
        }

        /// <summary>
        /// The using head.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingHead()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("head"));
            return this;
        }

        /// <summary>
        /// The using any verb.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingAnyVerb()
        {
            return this;
        }

        /// <summary>
        /// The using verb.
        /// </summary>
        /// <param name="verb">
        /// The verb.
        /// </param>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingVerb(string verb)
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher(verb));
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <returns>
        /// The <see cref="IRequestMatcher"/>.
        /// </returns>
        public IRequestMatcher WithBody(string body)
        {
            _requestMatchers.Add(new RequestMessageBodyMatcher(body));
            return this;
        }

        /// <summary>
        /// The with body byte[].
        /// </summary>
        /// <param name="body">
        /// The body as byte[].
        /// </param>
        /// <returns>
        /// The <see cref="IRequestMatcher"/>.
        /// </returns>
        public IRequestMatcher WithBody(byte[] body)
        {
            _requestMatchers.Add(new RequestMessageBodyMatcher(body));
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="func">
        /// The body function.
        /// </param>
        /// <returns>
        /// The <see cref="IRequestMatcher"/>.
        /// </returns>
        public IRequestMatcher WithBody(Func<string, bool> func)
        {
            _requestMatchers.Add(new RequestMessageBodyMatcher(func));
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="func">
        /// The body function.
        /// </param>
        /// <returns>
        /// The <see cref="IRequestMatcher"/>.
        /// </returns>
        public IRequestMatcher WithBody(Func<byte[], bool> func)
        {
            _requestMatchers.Add(new RequestMessageBodyMatcher(func));
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>
        /// The <see cref="IRequestMatcher" />.
        /// </returns>
        public IRequestMatcher WithBody(IMatcher matcher)
        {
            _requestMatchers.Add(new RequestMessageBodyMatcher(matcher));
            return this;
        }

        /// <summary>
        /// The with parameters.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="IRequestMatcher"/>.
        /// </returns>
        public IRequestMatcher WithParam(string key, params string[] values)
        {
            _requestMatchers.Add(new RequestMessageParamMatcher(key, values.ToList()));
            return this;
        }

        /// <summary>
        /// The with parameters.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        /// <returns>
        /// The <see cref="IRequestMatcher"/>.
        /// </returns>
        public IRequestMatcher WithParam(Func<IDictionary<string, List<string>>, bool> func)
        {
            _requestMatchers.Add(new RequestMessageParamMatcher(func));
            return this;
        }

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="ignoreCase">ignore Case</param>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder WithHeader(string name, string value, bool ignoreCase = true)
        {
            _requestMatchers.Add(new RequestMessageHeaderMatcher(name, value, ignoreCase));
            return this;
        }

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="func">
        ///     The func.
        /// </param>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder WithHeader(Func<IDictionary<string, string>, bool> func)
        {
            _requestMatchers.Add(new RequestMessageHeaderMatcher(func));
            return this;
        }
    }
}
