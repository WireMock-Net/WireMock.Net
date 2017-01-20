using System;
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
        /// The matcher.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The matcher.
        /// </summary>
        private readonly IMatcher _matcher;

        /// <summary>
        /// The path function
        /// </summary>
        private readonly Func<string, bool> _pathFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public RequestMessagePathMatcher([NotNull] string path)
        {
            Check.NotNull(path, nameof(path));
            _path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        public RequestMessagePathMatcher([NotNull] IMatcher matcher)
        {
            Check.NotNull(matcher, nameof(matcher));
            _matcher = matcher;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="func">
        /// The path func.
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
            if (_path != null)
                return string.CompareOrdinal(_path, requestMessage.Path) == 0;

            if (_matcher != null)
                return _matcher.IsMatch(requestMessage.Path);

            if (_pathFunc != null)
                return _pathFunc(requestMessage.Path);

            return false;
        }
    }
}