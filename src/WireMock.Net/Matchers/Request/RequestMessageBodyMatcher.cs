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
        /// The body as byte[].
        /// </summary>
        private readonly byte[] _bodyData;

        /// <summary>
        /// The body function
        /// </summary>
        public Func<string, bool> Func { get; }

        /// <summary>
        /// The body data function
        /// </summary>
        public Func<byte[], bool> DataFunc { get; }

        /// <summary>
        /// The matcher.
        /// </summary>
        public IMatcher Matcher { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="body">
        /// The body Regex pattern.
        /// </param>
        public RequestMessageBodyMatcher([NotNull] string body) : this(new SimMetricsMatcher(body))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="body">
        /// The body Regex pattern.
        /// </param>
        public RequestMessageBodyMatcher([NotNull] byte[] body)
        {
            Check.NotNull(body, nameof(body));
            _bodyData = body;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="func">
        /// The body func.
        /// </param>
        public RequestMessageBodyMatcher([NotNull] Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));
            Func = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="func">
        /// The body func.
        /// </param>
        public RequestMessageBodyMatcher([NotNull] Func<byte[], bool> func)
        {
            Check.NotNull(func, nameof(func));
            DataFunc = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="matcher">
        /// The body matcher.
        /// </param>
        public RequestMessageBodyMatcher([NotNull] IMatcher matcher)
        {
            Check.NotNull(matcher, nameof(matcher));
            Matcher = matcher;
        }

        /// <summary>
        /// Determines whether the specified RequestMessage is match.
        /// </summary>
        /// <param name="requestMessage">The RequestMessage.</param>
        /// <param name="requestMatchResult">The RequestMatchResult.</param>
        /// <returns>
        /// A value between 0.0 - 1.0 of the similarity.
        /// </returns>
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            double score = IsMatch(requestMessage);
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            if (Matcher != null)
                return Matcher.IsMatch(requestMessage.Body);

            if (_bodyData != null)
                return MatchScores.ToScore(requestMessage.BodyAsBytes == _bodyData);

            if (Func != null)
                return MatchScores.ToScore(requestMessage.Body != null && Func(requestMessage.Body));

            if (DataFunc != null && requestMessage.BodyAsBytes != null)
                return MatchScores.ToScore(requestMessage.BodyAsBytes != null && DataFunc(requestMessage.BodyAsBytes));

            return MatchScores.Mismatch;
        }
    }
}