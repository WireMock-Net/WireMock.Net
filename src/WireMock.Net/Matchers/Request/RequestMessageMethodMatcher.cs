using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request verb matcher.
    /// </summary>
    internal class RequestMessageMethodMatcher : IRequestMatcher
    {
        /// <summary>
        /// The methods
        /// </summary>
        public string[] Methods { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageMethodMatcher"/> class.
        /// </summary>
        /// <param name="methods">
        /// The verb.
        /// </param>
        public RequestMessageMethodMatcher([NotNull] params string[] methods)
        {
            Check.NotNull(methods, nameof(methods));
            Methods = methods.Select(v => v.ToLower()).ToArray();
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
            return Methods.Contains(requestMessage.Method);
        }
    }
}