using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using WireMock.Matchers;
using WireMock.Validation;

namespace WireMock
{
    /// <summary>
    /// The request body spec.
    /// </summary>
    public class RequestBodySpec : ISpecifyRequests
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
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="body">
        /// The body Regex pattern.
        /// </param>
        public RequestBodySpec([NotNull, RegexPattern] string body)
        {
            Check.NotNull(body, nameof(body));
            _matcher = new RegexMatcher(body);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="body">
        /// The body Regex pattern.
        /// </param>
        public RequestBodySpec([NotNull] byte[] body)
        {
            Check.NotNull(body, nameof(body));
            _bodyData = body;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="func">
        /// The body func.
        /// </param>
        public RequestBodySpec([NotNull] Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));
            _bodyFunc = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="func">
        /// The body func.
        /// </param>
        public RequestBodySpec([NotNull] Func<byte[], bool> func)
        {
            Check.NotNull(func, nameof(func));
            _bodyDataFunc = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="matcher">
        /// The body matcher.
        /// </param>
        public RequestBodySpec([NotNull] IMatcher matcher)
        {
            Check.NotNull(matcher, nameof(matcher));
            _matcher = matcher;
        }

        /// <summary>
        /// The is satisfied by.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSatisfiedBy(RequestMessage requestMessage)
        {
            if (_matcher != null)
                return _matcher.IsMatch(requestMessage.BodyAsString);

            if (_bodyData != null)
                return requestMessage.Body == _bodyData;

            if (_bodyFunc != null)
                return _bodyFunc(requestMessage.BodyAsString);

            if (_bodyDataFunc != null)
                return _bodyDataFunc(requestMessage.Body);

            return false;
        }
    }
}