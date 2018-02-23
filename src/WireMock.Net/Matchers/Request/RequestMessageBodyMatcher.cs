using System;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request body matcher.
    /// </summary>
    public class RequestMessageBodyMatcher : IRequestMatcher
    {
        /// <summary>
        /// The body function
        /// </summary>
        public Func<string, bool> Func { get; }

        /// <summary>
        /// The body data function for byte[]
        /// </summary>
        public Func<byte[], bool> DataFunc { get; }

        /// <summary>
        /// The body data function for json
        /// </summary>
        public Func<object, bool> JsonFunc { get; }

        /// <summary>
        /// The matcher.
        /// </summary>
        public IMatcher Matcher { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="body">The body.</param>
        public RequestMessageBodyMatcher([NotNull] string body) : this(new SimMetricsMatcher(body))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="body">The body.</param>
        public RequestMessageBodyMatcher([NotNull] byte[] body) : this(new ExactObjectMatcher(body))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="body">The body.</param>
        public RequestMessageBodyMatcher([NotNull] object body) : this(new ExactObjectMatcher(body))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public RequestMessageBodyMatcher([NotNull] Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));
            Func = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public RequestMessageBodyMatcher([NotNull] Func<byte[], bool> func)
        {
            Check.NotNull(func, nameof(func));
            DataFunc = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public RequestMessageBodyMatcher([NotNull] Func<object, bool> func)
        {
            Check.NotNull(func, nameof(func));
            JsonFunc = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        public RequestMessageBodyMatcher([NotNull] IMatcher matcher)
        {
            Check.NotNull(matcher, nameof(matcher));
            Matcher = matcher;
        }

        /// <see cref="IRequestMatcher.GetMatchingScore"/>
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            double score = IsMatch(requestMessage);
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            if (requestMessage.Body != null)
            {
                var stringMatcher = Matcher as IStringMatcher;
                if (stringMatcher != null)
                {
                    return stringMatcher.IsMatch(requestMessage.Body);
                }
            }

            var objectMatcher = Matcher as IObjectMatcher;
            if (objectMatcher != null)
            {
                if (requestMessage.BodyAsJson != null)
                {
                    return objectMatcher.IsMatch(requestMessage.BodyAsJson);
                }

                if (requestMessage.BodyAsBytes != null)
                {
                    return objectMatcher.IsMatch(requestMessage.BodyAsBytes);
                }
            }

            if (Func != null)
            {
                return MatchScores.ToScore(requestMessage.Body != null && Func(requestMessage.Body));
            }

            if (DataFunc != null)
            {
                return MatchScores.ToScore(requestMessage.BodyAsBytes != null && DataFunc(requestMessage.BodyAsBytes));
            }

            if (JsonFunc != null)
            {
                return MatchScores.ToScore(requestMessage.BodyAsJson != null && JsonFunc(requestMessage.BodyAsJson));
            }

            return MatchScores.Mismatch;
        }
    }
}