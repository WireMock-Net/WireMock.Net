using System;
using System.Collections.Generic;
using System.Linq;
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
        public IReadOnlyList<IMatcher> Matchers { get; }

        /// <summary>
        /// The path functions
        /// </summary>
        private readonly Func<string, bool>[] _pathFuncs;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        public RequestMessagePathMatcher([NotNull] params string[] paths) : this(paths.Select(path => new WildcardMatcher(path)).ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        public RequestMessagePathMatcher([NotNull] params IMatcher[] matchers)
        {
            Check.NotNull(matchers, nameof(matchers));
            Matchers = matchers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The path functions.</param>
        public RequestMessagePathMatcher([NotNull] params Func<string, bool>[] funcs)
        {
            Check.NotNull(funcs, nameof(funcs));
            _pathFuncs = funcs;
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
            if (Matchers != null)
                return Matchers.Any(matcher => matcher.IsMatch(requestMessage.Path));

            if (_pathFuncs != null)
                return _pathFuncs.Any(func => func(requestMessage.Path));

            return false;
        }
    }
}