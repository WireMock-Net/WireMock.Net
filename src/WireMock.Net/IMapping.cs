using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Matchers.Request;
using WireMock.ResponseProviders;

namespace WireMock
{
    /// <summary>
    /// The IMapping interface.
    /// </summary>
    public interface IMapping
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Gets the unique title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The full filename path for this mapping (only defined for static mappings).
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Scenario.
        /// </summary>
        [CanBeNull]
        string Scenario { get; }

        /// <summary>
        /// Execution state condition for the current mapping.
        /// </summary>
        [CanBeNull]
        string ExecutionConditionState { get; }

        /// <summary>
        /// The next state which will be signaled after the current mapping execution.
        /// In case the value is null, state will not be changed.
        /// </summary>
        [CanBeNull]
        string NextState { get; }

        /// <summary>
        /// The Request matcher.
        /// </summary>
        IRequestMatcher RequestMatcher { get; }

        /// <summary>
        /// The Provider.
        /// </summary>
        IResponseProvider Provider { get; }

        /// <summary>
        /// Is State started ?
        /// </summary>
        bool IsStartState { get; }

        /// <summary>
        /// Gets a value indicating whether this mapping is an Admin Interface.
        /// </summary>
        /// <value>
        /// <c>true</c> if this mapping is an Admin Interface; otherwise, <c>false</c>.
        /// </value>
        bool IsAdminInterface { get; }

        /// <summary>
        /// ResponseToAsync
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <returns>The <see cref="ResponseMessage"/>.</returns>
        Task<ResponseMessage> ResponseToAsync(RequestMessage requestMessage);

        /// <summary>
        /// Gets the RequestMatchResult based on the RequestMessage.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="nextState">The Next State.</param>
        /// <returns>The <see cref="RequestMatchResult"/>.</returns>
        RequestMatchResult GetRequestMatchResult(RequestMessage requestMessage, [CanBeNull] string nextState);
    }
}