using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Types;
using WireMock.Validation;

namespace WireMock.RequestBuilders
{
    public partial class Request
    {
        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour)"/>
        public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            return WithParam(key, false, matchBehaviour);
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, bool, MatchBehaviour)"/>
        public IRequestBuilder WithParam(string key, bool ignoreCase, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            Check.NotNull(key, nameof(key));

            _requestMatchers.Add(new RequestMessageParamMatcher(matchBehaviour, key, ignoreCase));
            return this;
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, string[])"/>
        public IRequestBuilder WithParam(string key, params string[] values)
        {
            return WithParam(key, MatchBehaviour.AcceptOnMatch, false, values);
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, bool, string[])"/>
        public IRequestBuilder WithParam(string key, bool ignoreCase, params string[] values)
        {
            return WithParam(key, MatchBehaviour.AcceptOnMatch, ignoreCase, values);
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, IStringMatcher[])"/>
        public IRequestBuilder WithParam(string key, params IStringMatcher[] matchers)
        {
            return WithParam(key, MatchBehaviour.AcceptOnMatch, false, matchers);
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, bool, IStringMatcher[])"/>
        public IRequestBuilder WithParam(string key, bool ignoreCase, params IStringMatcher[] matchers)
        {
            return WithParam(key, MatchBehaviour.AcceptOnMatch, ignoreCase, matchers);
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, string[])"/>
        public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, params string[] values)
        {
            return WithParam(key, matchBehaviour, false, values);
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, bool, string[])"/>
        public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, bool ignoreCase = false, params string[] values)
        {
            Check.NotNull(key, nameof(key));

            _requestMatchers.Add(new RequestMessageParamMatcher(matchBehaviour, key, ignoreCase, values));
            return this;
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, IStringMatcher[])"/>
        public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, params IStringMatcher[] matchers)
        {
            return WithParam(key, matchBehaviour, false, matchers);
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, bool, IStringMatcher[])"/>
        public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, bool ignoreCase, params IStringMatcher[] matchers)
        {
            Check.NotNull(key, nameof(key));

            _requestMatchers.Add(new RequestMessageParamMatcher(matchBehaviour, key, ignoreCase, matchers));
            return this;
        }

        /// <inheritdoc cref="IParamsRequestBuilder.WithParam(Func{IDictionary{string, WireMockList{string}}, bool}[])"/>
        public IRequestBuilder WithParam(params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs)
        {
            Check.NotNullOrEmpty(funcs, nameof(funcs));

            _requestMatchers.Add(new RequestMessageParamMatcher(funcs));
            return this;
        }
    }
}