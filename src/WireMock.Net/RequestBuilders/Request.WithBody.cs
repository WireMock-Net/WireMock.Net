using System;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Validation;

namespace WireMock.RequestBuilders
{
    public partial class Request
    {
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

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(IMatcher[])"/>
        public IRequestBuilder WithBody(IMatcher matcher)
        {
            return WithBody(new[] { matcher });
        }

        /// <inheritdoc cref="IBodyRequestBuilder.WithBody(IMatcher[])"/>
        public IRequestBuilder WithBody(IMatcher[] matchers)
        {
            Check.NotNull(matchers, nameof(matchers));

            _requestMatchers.Add(new RequestMessageBodyMatcher(matchers));
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
    }
}
