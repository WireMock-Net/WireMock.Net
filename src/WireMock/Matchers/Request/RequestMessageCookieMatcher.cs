using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request cookie matcher.
    /// </summary>
    public class RequestMessageCookieMatcher : IRequestMatcher
    {
        private readonly string _name;

        private readonly IMatcher _matcher;

        private readonly Func<IDictionary<string, string>, bool> _cookieFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignoreCase">The ignoreCase.</param>
        public RequestMessageCookieMatcher([NotNull] string name, [NotNull] string pattern, bool ignoreCase = true)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(pattern, nameof(pattern));

            _name = name;
            _matcher = new WildcardMatcher(pattern, ignoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        public RequestMessageCookieMatcher([NotNull] Func<IDictionary<string, string>, bool> func)
        {
            Check.NotNull(func, nameof(func));
            _cookieFunc = func;
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
            if (_cookieFunc != null)
                return _cookieFunc(requestMessage.Cookies);

            if (requestMessage.Cookies == null)
                return false;

            string headerValue = requestMessage.Cookies[_name];
            return _matcher.IsMatch(headerValue);
        }
    }
}