using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request parameters matcher.
    /// </summary>
    public class RequestMessageParamMatcher : IRequestMatcher
    {
        private readonly Func<IDictionary<string, WireMockList<string>>, bool>[] _funcs;

        /// <summary>
        /// The key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The values
        /// </summary>
        public IEnumerable<string> Values { get; }

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

            Key = key;
            Values = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
        /// </summary>
        /// <param name="funcs">The funcs.</param>
        public RequestMessageParamMatcher([NotNull] params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs)
        {
            Check.NotNull(funcs, nameof(funcs));
            _funcs = funcs;
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
            if (_funcs != null)
                return _funcs.Any(f => f(requestMessage.Query));

            var values = requestMessage.GetParameter(Key);
            return values?.Intersect(Values).Count() == Values.Count();
        }
    }
}