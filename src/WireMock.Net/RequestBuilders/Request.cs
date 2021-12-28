// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Stef.Validation;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The requests.
    /// </summary>
    public partial class Request : RequestMessageCompositeMatcher, IRequestBuilder
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
        /// Gets the request message matchers.
        /// </summary>
        /// <typeparam name="T">Type of IRequestMatcher</typeparam>
        /// <returns>A List{T}</returns>
        public IList<T> GetRequestMessageMatchers<T>() where T : IRequestMatcher
        {
            return new ReadOnlyCollection<T>(_requestMatchers.OfType<T>().ToList());
        }

        /// <summary>
        /// Gets the request message matcher.
        /// </summary>
        /// <typeparam name="T">Type of IRequestMatcher</typeparam>
        /// <returns>A RequestMatcher</returns>
        public T GetRequestMessageMatcher<T>() where T : IRequestMatcher
        {
            return _requestMatchers.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the request message matcher.
        /// </summary>
        /// <typeparam name="T">Type of IRequestMatcher</typeparam>
        /// <returns>A RequestMatcher</returns>
        public T GetRequestMessageMatcher<T>(Func<T, bool> func) where T : IRequestMatcher
        {
            return _requestMatchers.OfType<T>().FirstOrDefault(func);
        }

        /// <inheritdoc cref="IClientIPRequestBuilder.WithClientIP(IStringMatcher[])"/>
        public IRequestBuilder WithClientIP(params IStringMatcher[] matchers)
        {
            Guard.NotNullOrEmpty(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessageClientIPMatcher(matchers));
            return this;
        }

        /// <inheritdoc cref="IClientIPRequestBuilder.WithClientIP(string[])"/>
        public IRequestBuilder WithClientIP(params string[] clientIPs)
        {
            return WithClientIP(MatchBehaviour.AcceptOnMatch, clientIPs);
        }

        /// <inheritdoc cref="IClientIPRequestBuilder.WithClientIP(string[])"/>
        public IRequestBuilder WithClientIP(MatchBehaviour matchBehaviour, params string[] clientIPs)
        {
            Guard.NotNullOrEmpty(clientIPs, nameof(clientIPs));

            _requestMatchers.Add(new RequestMessageClientIPMatcher(matchBehaviour, clientIPs));
            return this;
        }

        /// <inheritdoc cref="IClientIPRequestBuilder.WithClientIP(Func{string, bool}[])"/>
        public IRequestBuilder WithClientIP(params Func<string, bool>[] funcs)
        {
            Guard.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageClientIPMatcher(funcs));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithPath(IStringMatcher[])"/>
        public IRequestBuilder WithPath(params IStringMatcher[] matchers)
        {
            Guard.NotNullOrEmpty(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessagePathMatcher(matchers));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithPath(string[])"/>
        public IRequestBuilder WithPath(params string[] paths)
        {
            return WithPath(MatchBehaviour.AcceptOnMatch, paths);
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithPath(MatchBehaviour, string[])"/>
        public IRequestBuilder WithPath(MatchBehaviour matchBehaviour, params string[] paths)
        {
            Guard.NotNullOrEmpty(paths, nameof(paths));

            _requestMatchers.Add(new RequestMessagePathMatcher(matchBehaviour, paths));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithPath(Func{string, bool}[])"/>
        public IRequestBuilder WithPath(params Func<string, bool>[] funcs)
        {
            Guard.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessagePathMatcher(funcs));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithUrl(IStringMatcher[])"/>
        public IRequestBuilder WithUrl(params IStringMatcher[] matchers)
        {
            Guard.NotNullOrEmpty(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessageUrlMatcher(matchers));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithUrl(string[])"/>
        public IRequestBuilder WithUrl(params string[] urls)
        {
            return WithUrl(MatchBehaviour.AcceptOnMatch, urls);
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithUrl(MatchBehaviour, string[])"/>
        public IRequestBuilder WithUrl(MatchBehaviour matchBehaviour, params string[] urls)
        {
            Guard.NotNullOrEmpty(urls, nameof(urls));

            _requestMatchers.Add(new RequestMessageUrlMatcher(matchBehaviour, urls));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithUrl(Func{string, bool}[])"/>
        public IRequestBuilder WithUrl(params Func<string, bool>[] funcs)
        {
            Guard.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageUrlMatcher(funcs));
            return this;
        }
    }
}