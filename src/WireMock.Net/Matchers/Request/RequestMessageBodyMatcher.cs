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
        /// The body.
        /// </summary>
        private readonly string _body;

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
        public RequestMessageBodyMatcher([NotNull] string body)
        {
            Check.NotNull(body, nameof(body));
            _body = body;
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
        /// <returns>
        ///   <c>true</c> if the specified RequestMessage is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(RequestMessage requestMessage)
        {
            if (Matcher != null)
                return Matcher.IsMatch(requestMessage.Body);

            if (_body != null)
                return requestMessage.Body == _body;

            if (_bodyData != null)
                return requestMessage.BodyAsBytes == _bodyData;

            if (Func != null)
                return Func(requestMessage.Body);

            if (DataFunc != null)
                return DataFunc(requestMessage.BodyAsBytes);

            return false;
        }
    }
}