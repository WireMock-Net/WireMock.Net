using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.ResponseProviders;
using WireMock.Settings;

namespace WireMock
{
    /// <summary>
    /// The Mapping.
    /// </summary>
    public class Mapping : IMapping
    {
        /// <inheritdoc />
        public Guid Guid { get; }

        /// <inheritdoc />
        public string Title { get; }

        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        public int Priority { get; }

        /// <inheritdoc />
        public string Scenario { get; }

        /// <inheritdoc />
        public string ExecutionConditionState { get; }

        /// <inheritdoc />
        public string NextState { get; }

        /// <inheritdoc />
        public int? StateTimes { get; }

        /// <inheritdoc />
        public IRequestMatcher RequestMatcher { get; }

        /// <inheritdoc />
        public IResponseProvider Provider { get; }

        /// <inheritdoc />
        public WireMockServerSettings Settings { get; }

        /// <inheritdoc />
        public bool IsStartState => Scenario == null || Scenario != null && NextState != null && ExecutionConditionState == null;

        /// <inheritdoc />
        public bool IsAdminInterface => Provider is DynamicResponseProvider || Provider is DynamicAsyncResponseProvider || Provider is ProxyAsyncResponseProvider;

        /// <inheritdoc />
        public bool LogMapping => !(Provider is DynamicResponseProvider || Provider is DynamicAsyncResponseProvider);

        /// <inheritdoc />
        public IWebhook[] Webhooks { get; }

        /// <inheritdoc />
        public ITimeSettings TimeSettings { get; }

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
        /// <param name="stateTimes">Only when the current state is executed this number, the next state which will occur. [Optional]</param>
        /// <param name="webhooks">The Webhooks. [Optional]</param>
        /// <param name="timeSettings">The TimeSettings. [Optional]</param>
        public Mapping(
            Guid guid,
            [CanBeNull] string title,
            [CanBeNull] string path,
            [NotNull] WireMockServerSettings settings,
            [NotNull] IRequestMatcher requestMatcher,
            [NotNull] IResponseProvider provider,
            int priority,
            [CanBeNull] string scenario,
            [CanBeNull] string executionConditionState,
            [CanBeNull] string nextState,
            [CanBeNull] int? stateTimes,
            [CanBeNull] IWebhook[] webhooks,
            [CanBeNull] ITimeSettings timeSettings)
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
            StateTimes = stateTimes;
            Webhooks = webhooks;
            TimeSettings = timeSettings;
        }

        /// <inheritdoc cref="IMapping.ProvideResponseAsync" />
        public Task<(ResponseMessage Message, IMapping Mapping)> ProvideResponseAsync(RequestMessage requestMessage)
        {
            return Provider.ProvideResponseAsync(requestMessage, Settings);
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