using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Matchers.Request;
using WireMock.ResponseProviders;
using WireMock.Settings;

namespace WireMock
{
    /// <summary>
    /// The Mapping.
    /// </summary>
    public class Mapping : IMapping
    {
        /// <inheritdoc cref="IMapping.Guid" />
        public Guid Guid { get; }

        /// <inheritdoc cref="IMapping.Title" />
        public string Title { get; }

        /// <inheritdoc cref="IMapping.Path" />
        public string Path { get; set; }

        /// <inheritdoc cref="IMapping.Priority" />
        public int Priority { get; }

        /// <inheritdoc cref="IMapping.Scenario" />
        public string Scenario { get; }

        /// <inheritdoc cref="IMapping.ExecutionConditionState" />
        public string ExecutionConditionState { get; }

        /// <inheritdoc cref="IMapping.NextState" />
        public string NextState { get; }

        /// <inheritdoc cref="IMapping.RequestMatcher" />
        public IRequestMatcher RequestMatcher { get; }

        /// <inheritdoc cref="IMapping.Provider" />
        public IResponseProvider Provider { get; }

        /// <inheritdoc cref="IMapping.Settings" />
        public WireMockServerSettings Settings { get; }

        /// <inheritdoc cref="IMapping.IsStartState" />
        public bool IsStartState => Scenario == null || Scenario != null && NextState != null && ExecutionConditionState == null;

        /// <inheritdoc cref="IMapping.IsAdminInterface" />
        public bool IsAdminInterface => Provider is DynamicResponseProvider || Provider is DynamicAsyncResponseProvider || Provider is ProxyAsyncResponseProvider;

        /// <inheritdoc cref="IMapping.LogMapping" />
        public bool LogMapping => !(Provider is DynamicResponseProvider || Provider is DynamicAsyncResponseProvider);

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="title">The unique title (can be null).</param>
        /// <param name="path">The full file path from this mapping title (can be null).</param>
        /// <param name="settings">The WireMockServerSettings.</param>
        /// <param name="requestMatcher">The request matcher.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="priority">The priority for this mapping.</param>
        /// <param name="scenario">The scenario. [Optional]</param>
        /// <param name="executionConditionState">State in which the current mapping can occur. [Optional]</param>
        /// <param name="nextState">The next state which will occur after the current mapping execution. [Optional]</param>
        public Mapping(Guid guid, [CanBeNull] string title, [CanBeNull] string path,
            [NotNull] WireMockServerSettings settings, [NotNull] IRequestMatcher requestMatcher, [NotNull] IResponseProvider provider,
            int priority, [CanBeNull] string scenario, [CanBeNull] string executionConditionState, [CanBeNull] string nextState)
        {
            Guid = guid;
            Title = title;
            Path = path;
            Settings = settings;
            RequestMatcher = requestMatcher;
            Provider = provider;
            Priority = priority;
            Scenario = scenario;
            ExecutionConditionState = executionConditionState;
            NextState = nextState;
        }

        /// <inheritdoc cref="IMapping.ProvideResponseAsync" />
        public async Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage)
        {
            return await Provider.ProvideResponseAsync(requestMessage, Settings);
        }

        /// <inheritdoc cref="IMapping.GetRequestMatchResult" />
        public RequestMatchResult GetRequestMatchResult(RequestMessage requestMessage, string nextState)
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
    }
}