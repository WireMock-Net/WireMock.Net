using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request url matcher.
    /// </summary>
    public class RequestMessageUrlMatcher : IRequestMatcher
    {
        /// <summary>
        /// The urlRegex.
        /// </summary>
        private readonly Regex _urlRegex;

        /// <summary>
        /// The url function
        /// </summary>
        private readonly Func<string, bool> _urlFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
        /// </summary>
        /// <param name="url">
        /// The url Regex pattern.
        /// </param>
        public RequestMessageUrlMatcher([NotNull, RegexPattern] string url)
        {
            Check.NotNull(url, nameof(url));
            _urlRegex = new Regex(url);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
        /// </summary>
        /// <param name="func">
        /// The url func.
        /// </param>
        public RequestMessageUrlMatcher(Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));
            _urlFunc = func;
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
            return _urlRegex?.IsMatch(requestMessage.Url) ?? _urlFunc(requestMessage.Url);
        }
    }
}