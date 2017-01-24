using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly IReadOnlyList<IMatcher> Matchers;

        /// <summary>
        /// The url functions
        /// </summary>
        private readonly Func<string, bool>[] _urlFuncs;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
        /// </summary>
        /// <param name="urls">The urls.</param>
        public RequestMessageUrlMatcher([NotNull] params string[] urls) : this(urls.Select(url => new WildcardMatcher(url)).ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        public RequestMessageUrlMatcher([NotNull] params IMatcher[] matchers)
        {
            Check.NotNull(matchers, nameof(matchers));
            Matchers = matchers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The url functions.</param>
        public RequestMessageUrlMatcher([NotNull] params Func<string, bool>[] funcs)
        {
            Check.NotNull(funcs, nameof(funcs));
            _urlFuncs = funcs;
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

            if (_urlFuncs != null)
                return _urlFuncs.Any(func => func(requestMessage.Url));

            return false;
        }
    }
}