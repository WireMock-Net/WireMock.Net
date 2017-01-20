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
        /// The bodyRegex.
        /// </summary>
        private readonly byte[] _bodyData;

        /// <summary>
        /// The matcher.
        /// </summary>
        private readonly IMatcher _matcher;

        /// <summary>
        /// The body function
        /// </summary>
        private readonly Func<string, bool> _bodyFunc;

        /// <summary>
        /// The body data function
        /// </summary>
        private readonly Func<byte[], bool> _bodyDataFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
        /// </summary>
        /// <param name="body">
        /// The body Regex pattern.
        /// </param>
        public RequestMessageBodyMatcher([NotNull, RegexPattern] string body)
        {
            Check.NotNull(body, nameof(body));
            _matcher = new RegexMatcher(body);
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
            _bodyFunc = func;
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
            _bodyDataFunc = func;
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
            _matcher = matcher;
        }

        /// <summary>
        /// Determines whether the specified RequestMessage is match.
        /// </summary>
        /// <param name="requestMessage">The RequestMessage.</param>
        /// <returns>
        ///   <c>true</c> if the specified RequestMessage is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(RequestMessage requestMessage)
        {
            if (_matcher != null)
                return _matcher.IsMatch(requestMessage.Body);

            if (_bodyData != null)
                return requestMessage.BodyAsBytes == _bodyData;

            if (_bodyFunc != null)
                return _bodyFunc(requestMessage.Body);

            if (_bodyDataFunc != null)
                return _bodyDataFunc(requestMessage.BodyAsBytes);

            return false;
        }
    }
}