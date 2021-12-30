// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System.Linq;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Stef.Validation;

namespace WireMock.RequestBuilders
{
    public partial class Request
    {
        /// <inheritdoc cref="IMethodRequestBuilder.UsingConnect(MatchBehaviour)"/>
        public IRequestBuilder UsingConnect(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.CONNECT));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingDelete(MatchBehaviour)"/>
        public IRequestBuilder UsingDelete(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.DELETE));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingGet(MatchBehaviour)"/>
        public IRequestBuilder UsingGet(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.GET));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingHead(MatchBehaviour)"/>
        public IRequestBuilder UsingHead(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.HEAD));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingOptions(MatchBehaviour)"/>
        public IRequestBuilder UsingOptions(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.OPTIONS));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingPost(MatchBehaviour)"/>
        public IRequestBuilder UsingPost(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.POST));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingPatch(MatchBehaviour)"/>
        public IRequestBuilder UsingPatch(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.PATCH));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingPut(MatchBehaviour)"/>
        public IRequestBuilder UsingPut(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.PUT));
            return this;
        }

        /// <inheritdoc cref="IMethodRequestBuilder.UsingTrace(MatchBehaviour)"/>
        public IRequestBuilder UsingTrace(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, HttpRequestMethods.TRACE));
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
            Guard.NotNullOrEmpty(methods, nameof(methods));

            _requestMatchers.Add(new RequestMessageMethodMatcher(matchBehaviour, methods));
            return this;
        }
    }
}