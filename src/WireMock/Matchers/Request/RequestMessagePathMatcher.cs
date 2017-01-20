using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request path matcher.
    /// </summary>
    public class RequestMessagePathMatcher : IRequestMatcher
    {
        /// <summary>
        /// The pathRegex.
        /// </summary>
        private readonly Regex _pathRegex;

        /// <summary>
        /// The url function
        /// </summary>
        private readonly Func<string, bool> _pathFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="path">
        /// The path Regex pattern.
        /// </param>
        public RequestMessagePathMatcher([NotNull, RegexPattern] string path)
        {
            Check.NotNull(path, nameof(path));
            _pathRegex = new Regex(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="func">
        /// The url func.
        /// </param>
        public RequestMessagePathMatcher([NotNull] Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));
            _pathFunc = func;
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
            return _pathRegex?.IsMatch(requestMessage.Path) ?? _pathFunc(requestMessage.Path);
        }
    }
}