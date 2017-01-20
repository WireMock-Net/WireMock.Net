using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request parameters matcher.
    /// </summary>
    public class RequestMessageParamMatcher : IRequestMatcher
    {
        /// <summary>
        /// The _key.
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// The _values.
        /// </summary>
        private readonly IEnumerable<string> _values;

        private readonly Func<IDictionary<string, List<string>>, bool> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public RequestMessageParamMatcher([NotNull] string key, [NotNull] IEnumerable<string> values)
        {
            Check.NotNull(key, nameof(key));
            Check.NotNull(values, nameof(values));

            _key = key;
            _values = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        public RequestMessageParamMatcher([NotNull] Func<IDictionary<string, List<string>>, bool> func)
        {
            Check.NotNull(func, nameof(func));
            _func = func;
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
            if (_func != null)
            {
                return _func(requestMessage.Parameters);
            }

            return requestMessage.GetParameter(_key).Intersect(_values).Count() == _values.Count();
        }
    }
}