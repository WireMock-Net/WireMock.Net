using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Matchers.Request;

namespace WireMock
{
    /// <summary>
    /// The Mapping.
    /// </summary>
    public class Mapping
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Gets the unique title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Scenario.
        /// </summary>
        [CanBeNull]
        public string Scenario { get; }

        /// <summary>
        /// Execution state condition for the current mapping.
        /// </summary>
        [CanBeNull]
        public object ExecutionConditionState { get; }

        /// <summary>
        /// The next state which will be signaled after the current mapping execution.
        /// In case the value is null state will not be changed.
        /// </summary>
        [CanBeNull]
        public object NextState { get; }

        /// <summary>
        /// The Request matcher.
        /// </summary>
        public IRequestMatcher RequestMatcher { get; }

        /// <summary>
        /// The Provider.
        /// </summary>
        public IResponseProvider Provider { get; }

        /// <summary>
        /// Is State started ?
        /// </summary>
        public bool IsStartState => Scenario == null || Scenario != null && NextState != null && ExecutionConditionState == null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="title">The unique title (can be null_.</param>
        /// <param name="requestMatcher">The request matcher.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="priority">The priority for this mapping.</param>
        /// <param name="scenario">The scenario. [Optional]</param>
        /// <param name="executionConditionState">State in which the current mapping can occur. [Optional]</param>
        /// <param name="nextState">The next state which will occur after the current mapping execution. [Optional]</param>
        public Mapping(Guid guid, [CanBeNull] string title, IRequestMatcher requestMatcher, IResponseProvider provider, int priority, [CanBeNull] string scenario, [CanBeNull] object executionConditionState, [CanBeNull] object nextState)
        {
            Guid = guid;
            Title = title;
            RequestMatcher = requestMatcher;
            Provider = provider;
            Priority = priority;
            Scenario = scenario;
            ExecutionConditionState = executionConditionState;
            NextState = nextState;
        }

        /// <summary>
        /// The response to.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <returns>The <see cref="ResponseMessage"/>.</returns>
        public async Task<ResponseMessage> ResponseToAsync(RequestMessage requestMessage)
        {
            return await Provider.ProvideResponseAsync(requestMessage);
        }

        /// <summary>
        /// Gets the RequestMatchResult based on the RequestMessage.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="nextState">The Next State.</param>
        /// <returns>The <see cref="RequestMatchResult"/>.</returns>
        public RequestMatchResult GetRequestMatchResult(RequestMessage requestMessage, [CanBeNull] object nextState)
        {
            var result = new RequestMatchResult();

            RequestMatcher.GetMatchingScore(requestMessage, result);

            // Only check state if Scenario is defined
            if (Scenario != null)
            {
                var matcher = new RequestMessageScenarioAndStateMatcher(nextState, ExecutionConditionState);
                matcher.GetMatchingScore(requestMessage, result);
                //// If ExecutionConditionState is null, this means that request is the start from a scenario. So just return.
                //if (ExecutionConditionState != null)
                //{
                //    // ExecutionConditionState is not null, so get score for matching with the nextState.
                //    var matcher = new RequestMessageScenarioAndStateMatcher(nextState, ExecutionConditionState);
                //    matcher.GetMatchingScore(requestMessage, result);
                //}
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether this mapping is an Admin Interface.
        /// </summary>
        /// <value>
        /// <c>true</c> if this mapping is an Admin Interface; otherwise, <c>false</c>.
        /// </value>
        public bool IsAdminInterface => Provider is DynamicResponseProvider || Provider is DynamicAsyncResponseProvider || Provider is ProxyAsyncResponseProvider;
    }
}