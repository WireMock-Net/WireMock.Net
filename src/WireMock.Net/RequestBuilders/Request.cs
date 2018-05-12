using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Util;
using WireMock.Validation;

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
        /// Gets the request message matchers.
        /// </summary>
        /// <typeparam name="T">Type of IRequestMatcher</typeparam>
        /// <returns>A List{T}</returns>
        public IList<T> GetRequestMessageMatchers<T>() where T : IRequestMatcher
        {
            return new ReadOnlyCollection<T>(_requestMatchers.Where(rm => rm is T).Cast<T>().ToList());
        }

        /// <summary>
        /// Gets the request message matcher.
        /// </summary>
        /// <typeparam name="T">Type of IRequestMatcher</typeparam>
        /// <returns>A RequestMatcher</returns>
        public T GetRequestMessageMatcher<T>() where T : IRequestMatcher
        {
            return _requestMatchers.Where(rm => rm is T).Cast<T>().FirstOrDefault();
        }

        /// <inheritdoc cref="IClientIPRequestBuilder.WithClientIP(IStringMatcher[])"/>
        public IRequestBuilder WithClientIP(params IStringMatcher[] matchers)
        {
            Check.NotNullOrEmpty(matchers, nameof(matchers));

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
            Check.NotNullOrEmpty(clientIPs, nameof(clientIPs));

            _requestMatchers.Add(new RequestMessageClientIPMatcher(matchBehaviour, clientIPs));
            return this;
        }

        /// <inheritdoc cref="IClientIPRequestBuilder.WithClientIP(Func{string, bool}[])"/>
        public IRequestBuilder WithClientIP(params Func<string, bool>[] funcs)
        {
            Check.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageClientIPMatcher(funcs));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithPath(IStringMatcher[])"/>
        public IRequestBuilder WithPath(params IStringMatcher[] matchers)
        {
            Check.NotNullOrEmpty(matchers, nameof(matchers));

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
            Check.NotNullOrEmpty(paths, nameof(paths));

            _requestMatchers.Add(new RequestMessagePathMatcher(matchBehaviour, paths));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithPath(Func{string, bool}[])"/>
        public IRequestBuilder WithPath(params Func<string, bool>[] funcs)
        {
            Check.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessagePathMatcher(funcs));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithUrl(IStringMatcher[])"/>
        public IRequestBuilder WithUrl(params IStringMatcher[] matchers)
        {
            Check.NotNullOrEmpty(matchers, nameof(matchers));

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
            Check.NotNullOrEmpty(urls, nameof(urls));

            _requestMatchers.Add(new RequestMessageUrlMatcher(matchBehaviour, urls));
            return this;
        }

        /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithUrl(Func{string, bool}[])"/>
        public IRequestBuilder WithUrl(params Func<string, bool>[] funcs)
        {
            Check.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageUrlMatcher(funcs));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingDelete(MatchBehaviour)"/>
        public IRequestBuilder UsingDelete(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, "delete"));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingGet(MatchBehaviour)"/>
        public IRequestBuilder UsingGet(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, "get"));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingHead(MatchBehaviour)"/>
        public IRequestBuilder UsingHead(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, "head"));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingPost(MatchBehaviour)"/>
        public IRequestBuilder UsingPost(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, "post"));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingPatch(MatchBehaviour)"/>
        public IRequestBuilder UsingPatch(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, "patch"));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingPut(MatchBehaviour)"/>
        public IRequestBuilder UsingPut(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, "put"));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingAnyMethod"/>
        public IRequestBuilder UsingAnyMethod()
        {
            var matchers = _requestMatchers.Where(m => m is RequestMessageMethodMatcher).ToList();
            foreach (var matcher in matchers)
            {
                _requestMatchers.Remove(matcher);
            }

            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingAnyVerb"/>
        public IRequestBuilder UsingAnyVerb()
        {
            return UsingAnyMethod();
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingMethod(string[])"/>
        public IRequestBuilder UsingMethod(params string[] methods)
        {
            return UsingMethod(MatchBehaviour.AcceptOnMatch, methods);
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingVerb(string[])"/>
        public IRequestBuilder UsingVerb(params string[] verbs)
        {
            return UsingMethod(verbs);
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingMethod(MatchBehaviour, string[])"/>
        public IRequestBuilder UsingMethod(MatchBehaviour matchBehaviour, params string[] methods)
        {
            Check.NotNullOrEmpty(methods, nameof(methods));

            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, methods));
            return this;
        }

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(string, MatchBehaviour)"/>
        public IRequestBuilder WithBody(string body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageBodyMatcher(matchBehaviour, body));
            return this;
        }

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(byte[], MatchBehaviour)"/>
        public IRequestBuilder WithBody(byte[] body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageBodyMatcher(matchBehaviour, body));
            return this;
        }

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(object, MatchBehaviour)"/>
        public IRequestBuilder WithBody(object body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageBodyMatcher(matchBehaviour, body));
            return this;
        }

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(IMatcher)"/>
        public IRequestBuilder WithBody(IMatcher matcher)
        {
            Check.NotNull(matcher, nameof(matcher));

            _requestMatchers.Add(new RequestMessageBodyMatcher(matcher));
            return this;
        }

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(Func{string, bool})"/>
        public IRequestBuilder WithBody(Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));

            _requestMatchers.Add(new RequestMessageBodyMatcher(func));
            return this;
        }

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(Func{byte[], bool})"/>
        public IRequestBuilder WithBody(Func<byte[], bool> func)
        {
            Check.NotNull(func, nameof(func));

            _requestMatchers.Add(new RequestMessageBodyMatcher(func));
            return this;
        }

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(Func{object, bool})"/>
        public IRequestBuilder WithBody(Func<object, bool> func)
        {
            Check.NotNull(func, nameof(func));

            _requestMatchers.Add(new RequestMessageBodyMatcher(func));
            return this;
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour)"/>
        public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            Check.NotNull(key, nameof(key));

            _requestMatchers.Add(new RequestMessageParamMatcher(matchBehaviour, key));
            return this;
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, string[])"/>
        public IRequestBuilder WithParam(string key, params string[] values)
        {
            return WithParam(key, MatchBehaviour.AcceptOnMatch, values);
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, string[])"/>
        public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, params string[] values)
        {
            Check.NotNull(key, nameof(key));

            _requestMatchers.Add(new RequestMessageParamMatcher(matchBehaviour, key, values));
            return this;
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(Func{IDictionary{string, WireMockList{string}}, bool}[])"/>
        public IRequestBuilder WithParam(params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs)
        {
            Check.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageParamMatcher(funcs));
            return this;
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithHeader(string, string, MatchBehaviour)"/>
        public IRequestBuilder WithHeader(string name, string pattern, MatchBehaviour matchBehaviour)
        {
            return WithHeader(name, pattern, true, matchBehaviour);
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithHeader(string, string, bool, MatchBehaviour)"/>
        public IRequestBuilder WithHeader(string name, string pattern, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(pattern, nameof(pattern));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(matchBehaviour, name, pattern, ignoreCase));
            return this;
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithHeader(string, string[], MatchBehaviour)"/>
        public IRequestBuilder WithHeader(string name, string[] patterns, MatchBehaviour matchBehaviour)
        {
            return WithHeader(name, patterns, true, matchBehaviour);
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithHeader(string, string[], bool, MatchBehaviour)"/>
        public IRequestBuilder WithHeader(string name, string[] patterns, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(patterns, nameof(patterns));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(matchBehaviour, name, patterns, ignoreCase));
            return this;
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithHeader(string, IStringMatcher[])"/>
        public IRequestBuilder WithHeader(string name, params IStringMatcher[] matchers)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNullOrEmpty(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(name, matchers));
            return this;
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithHeader(Func{IDictionary{string, string[]}, bool}[])"/>
        public IRequestBuilder WithHeader(params Func<IDictionary<string, string[]>, bool>[] funcs)
        {
            Check.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(funcs));
            return this;
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithCookie(string, string, bool, MatchBehaviour)"/>
        public IRequestBuilder WithCookie(string name, string pattern, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageCookieMatcher(matchBehaviour, name, pattern, ignoreCase));
            return this;
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithCookie(string, IStringMatcher[])"/>
        public IRequestBuilder WithCookie(string name, params IStringMatcher[] matchers)
        {
            Check.NotNullOrEmpty(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessageCookieMatcher(name, matchers));
            return this;
        }

        /// <inheritdoc cref="IHeadersAndCookiesRequestBuilder.WithCookie(Func{IDictionary{string, string}, bool}[])"/>
        public IRequestBuilder WithCookie(params Func<IDictionary<string, string>, bool>[] funcs)
        {
            Check.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageCookieMatcher(funcs));
            return this;
        }
    }
}