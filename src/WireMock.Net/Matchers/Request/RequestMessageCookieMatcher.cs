using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request cookie matcher.
    /// </summary>
    public class RequestMessageCookieMatcher : IRequestMatcher
    {
        /// <value>
        /// The funcs.
        /// </value>
        public Func<IDictionary<string, string>, bool>[] Funcs { get; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; }

        /// <value>
        /// The matchers.
        /// </value>
        public IMatcher[] Matchers { get; }
        
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

            Name = name;
            Matchers = new IMatcher[] { new WildcardMatcher(pattern, ignoreCase) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchers">The matchers.</param>
        public RequestMessageCookieMatcher([NotNull] string name, [NotNull] params IMatcher[] matchers)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(matchers, nameof(matchers));

            Name = name;
            Matchers = matchers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        public RequestMessageCookieMatcher([NotNull] params Func<IDictionary<string, string>, bool>[] funcs)
        {
            Check.NotNull(funcs, nameof(funcs));

            Funcs = funcs;
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
            if (Funcs != null)
                return Funcs.Any(cf => cf(requestMessage.Cookies));

            if (requestMessage.Cookies == null)
                return false;

            string headerValue = requestMessage.Cookies[Name];
            return Matchers.Any(m => m.IsMatch(headerValue));
        }
    }
}