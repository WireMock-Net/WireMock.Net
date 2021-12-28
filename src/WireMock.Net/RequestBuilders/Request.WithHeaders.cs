// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Stef.Validation;

namespace WireMock.RequestBuilders
{
    public partial class Request
    {
        /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, string, MatchBehaviour)"/>
        public IRequestBuilder WithHeader(string name, string pattern, MatchBehaviour matchBehaviour)
        {
            return WithHeader(name, pattern, true, matchBehaviour);
        }

        /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, string, bool, MatchBehaviour)"/>
        public IRequestBuilder WithHeader(string name, string pattern, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            Guard.NotNull(name, nameof(name));
            Guard.NotNull(pattern, nameof(pattern));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(matchBehaviour, name, pattern, ignoreCase));
            return this;
        }

        /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, string[], MatchBehaviour)"/>
        public IRequestBuilder WithHeader(string name, string[] patterns, MatchBehaviour matchBehaviour)
        {
            return WithHeader(name, patterns, true, matchBehaviour);
        }

        /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, string[], bool, MatchBehaviour)"/>
        public IRequestBuilder WithHeader(string name, string[] patterns, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            Guard.NotNull(name, nameof(name));
            Guard.NotNull(patterns, nameof(patterns));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(matchBehaviour, name, ignoreCase, patterns));
            return this;
        }

        /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, IStringMatcher[])"/>
        public IRequestBuilder WithHeader(string name, params IStringMatcher[] matchers)
        {
            Guard.NotNull(name, nameof(name));
            Guard.NotNullOrEmpty(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, name, false, matchers));
            return this;
        }

        /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, bool, IStringMatcher[])"/>
        public IRequestBuilder WithHeader(string name, bool ignoreCase, params IStringMatcher[] matchers)
        {
            Guard.NotNull(name, nameof(name));
            Guard.NotNullOrEmpty(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, name, ignoreCase, matchers));
            return this;
        }

        /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, IStringMatcher[])"/>
        public IRequestBuilder WithHeader(string name, bool ignoreCase, MatchBehaviour matchBehaviour, params IStringMatcher[] matchers)
        {
            Guard.NotNull(name, nameof(name));
            Guard.NotNullOrEmpty(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(matchBehaviour, name, ignoreCase, matchers));
            return this;
        }

        /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(Func{IDictionary{string, string[]}, bool}[])"/>
        public IRequestBuilder WithHeader(params Func<IDictionary<string, string[]>, bool>[] funcs)
        {
            Guard.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageHeaderMatcher(funcs));
            return this;
        }
    }
}