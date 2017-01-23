using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Util;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The requests.
    /// </summary>
    public class Request : RequestMessageCompositeMatcher, IRequestBuilder
    {
        private readonly IList<IRequestMatcher> _requestMatchers;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        public static IRequestBuilder Create()
        {
            return new Request(new List<IRequestMatcher>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="requestMatchers">The request matchers.</param>
        private Request(IList<IRequestMatcher> requestMatchers) : base(requestMatchers)
        {
            _requestMatchers = requestMatchers;
        }

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        public IUrlAndPathRequestBuilder WithUrl(IMatcher matcher)
        {
            _requestMatchers.Add(new RequestMessageUrlMatcher(matcher));

            return this;
        }

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        public IUrlAndPathRequestBuilder WithUrl(params string[] urls)
        {
            var or = new RequestMessageCompositeMatcher(urls.Select(url => new RequestMessageUrlMatcher(url)), CompositeMatcherType.Or);
            _requestMatchers.Add(or);

            return this;
        }

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="funcs">The url func.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        public IUrlAndPathRequestBuilder WithUrl(params Func<string, bool>[] funcs)
        {
            var or = new RequestMessageCompositeMatcher(funcs.Select(func => new RequestMessageUrlMatcher(func)), CompositeMatcherType.Or);
            _requestMatchers.Add(or);

            return this;
        }

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        public IUrlAndPathRequestBuilder WithPath(IMatcher matcher)
        {
            _requestMatchers.Add(new RequestMessagePathMatcher(matcher));

            return this;
        }

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="paths">The path.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        public IUrlAndPathRequestBuilder WithPath(params string[] paths)
        {
            var or = new RequestMessageCompositeMatcher(paths.Select(path => new RequestMessageUrlMatcher(path)), CompositeMatcherType.Or);
            _requestMatchers.Add(or);

            return this;
        }

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="funcs">The path func.</param>
        /// <returns>The <see cref="IUrlAndPathRequestBuilder"/>.</returns>
        public IUrlAndPathRequestBuilder WithPath(params Func<string, bool>[] funcs)
        {
            var or = new RequestMessageCompositeMatcher(funcs.Select(func => new RequestMessageUrlMatcher(func)), CompositeMatcherType.Or);
            _requestMatchers.Add(or);

            return this;
        }

        /// <summary>
        /// The using get.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        public IHeadersAndCookiesRequestBuilder UsingGet()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("get"));
            return this;
        }

        /// <summary>
        /// The using post.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        public IHeadersAndCookiesRequestBuilder UsingPost()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("post"));
            return this;
        }

        /// <summary>
        /// The using put.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        public IHeadersAndCookiesRequestBuilder UsingPut()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("put"));
            return this;
        }

        /// <summary>
        /// The using delete.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        public IHeadersAndCookiesRequestBuilder UsingDelete()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("delete"));
            return this;
        }

        /// <summary>
        /// The using head.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        public IHeadersAndCookiesRequestBuilder UsingHead()
        {
            _requestMatchers.Add(new RequestMessageVerbMatcher("head"));
            return this;
        }

        /// <summary>
        /// The using any verb.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        public IHeadersAndCookiesRequestBuilder UsingAnyVerb()
        {
            var matchers = _requestMatchers.Where(m => m is RequestMessageVerbMatcher).ToList();
            foreach (var matcher in matchers)
            {
                _requestMatchers.Remove(matcher);
            }

            return this;
        }

        /// <summary>
        /// The using verb.
        /// </summary>
        /// <param name="verbs">The verbs.</param>
        /// <returns>The <see cref="IHeadersAndCookiesRequestBuilder"/>.</returns>
        public IHeadersAndCookiesRequestBuilder UsingVerb(params string[] verbs)
        {
            var or = new RequestMessageCompositeMatcher(verbs.Select(verb => new RequestMessageVerbMatcher(verb)), CompositeMatcherType.Or);
            _requestMatchers.Add(or);

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
        public IRequestMatcher WithParam(Func<IDictionary<string, WireMockList<string>>, bool> func)
        {
            _requestMatchers.Add(new RequestMessageParamMatcher(func));
            return this;
        }

        /// <summary>
        /// With header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public IHeadersAndCookiesRequestBuilder WithHeader(string name, string pattern, bool ignoreCase = true)
        {
            _requestMatchers.Add(new RequestMessageHeaderMatcher(name, pattern, ignoreCase));
            return this;
        }

        /// <summary>
        /// With header.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        /// <returns></returns>
        public IHeadersAndCookiesRequestBuilder WithHeader(params Func<IDictionary<string, string>, bool>[] funcs)
        {
            var or = new RequestMessageCompositeMatcher(funcs.Select(func => new RequestMessageHeaderMatcher(func)), CompositeMatcherType.Or);
            _requestMatchers.Add(or);

            return this;
        }

        /// <summary>
        /// With cookie.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public IHeadersAndCookiesRequestBuilder WithCookie(string name, string pattern, bool ignoreCase = true)
        {
            _requestMatchers.Add(new RequestMessageCookieMatcher(name, pattern, ignoreCase));
            return this;
        }

        /// <summary>
        /// With header.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        /// <returns></returns>
        public IHeadersAndCookiesRequestBuilder WithCookie(params Func<IDictionary<string, string>, bool>[] funcs)
        {
            var or = new RequestMessageCompositeMatcher(funcs.Select(func => new RequestMessageCookieMatcher(func)), CompositeMatcherType.Or);
            _requestMatchers.Add(or);

            return this;
        }
    }
}