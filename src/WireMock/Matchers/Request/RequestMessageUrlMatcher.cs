using System;
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
        /// The matcher.
        /// </summary>
        private readonly IMatcher _matcher;

        /// <summary>
        /// The url function
        /// </summary>
        private readonly Func<string, bool> _urlFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
        /// </summary>
        /// <param name="url">The url.</param>
        public RequestMessageUrlMatcher([NotNull] string url) : this(new WildcardMatcher(url))
        {
            _matcher = new WildcardMatcher(url);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        public RequestMessageUrlMatcher([NotNull] IMatcher matcher)
        {
            Check.NotNull(matcher, nameof(matcher));
            _matcher = matcher;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
        /// </summary>
        /// <param name="func">
        /// The url func.
        /// </param>
        public RequestMessageUrlMatcher([NotNull] Func<string, bool> func)
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
            if (_matcher != null)
                return _matcher.IsMatch(requestMessage.Path);

            if (_urlFunc != null)
                return _urlFunc(requestMessage.Url);

            return false;
        }
    }
}