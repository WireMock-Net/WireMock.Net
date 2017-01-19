using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock
{
    /// <summary>
    /// The request path spec.
    /// </summary>
    public class RequestPathSpec : ISpecifyRequests
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
        /// Initializes a new instance of the <see cref="RequestPathSpec"/> class.
        /// </summary>
        /// <param name="path">
        /// The path Regex pattern.
        /// </param>
        public RequestPathSpec([NotNull, RegexPattern] string path)
        {
            Check.NotNull(path, nameof(path));
            _pathRegex = new Regex(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestPathSpec"/> class.
        /// </summary>
        /// <param name="func">
        /// The url func.
        /// </param>
        public RequestPathSpec([NotNull] Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));
            _pathFunc = func;
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
            return _pathRegex?.IsMatch(requestMessage.Path) ?? _pathFunc(requestMessage.Path);
        }
    }
}