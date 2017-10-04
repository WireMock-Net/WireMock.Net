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
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid Guid { get; }

        /// <summary>
        /// Gets the unique title.
        /// </summary>
        /// <value>
        /// The unique title.
        /// </value>
        public string Title { get; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; }

        /// <summary>
        /// Execution state condition for the current mapping.
        /// </summary>
        public object ExecutionConditionState { get; }

        /// <summary>
        /// The next state which will be signaled after the current mapping execution.
        /// In case the value is null state will not be changed.
        /// </summary>
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
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="title">The unique title (can be null_.</param>
        /// <param name="requestMatcher">The request matcher.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="priority">The priority for this mapping.</param>
        public Mapping(Guid guid, [CanBeNull] string title, IRequestMatcher requestMatcher, IResponseProvider provider, int priority)
            : this(guid, title, requestMatcher, provider, priority, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="title">The unique title (can be null_.</param>
        /// <param name="requestMatcher">The request matcher.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="priority">The priority for this mapping.</param>
        /// <param name="executionConditionState">State in which the current mapping can occur. Happens if not null</param>
        /// <param name="nextState">The next state which will occur after the current mapping execution. Happens if not null</param>
        public Mapping(Guid guid, [CanBeNull] string title, IRequestMatcher requestMatcher, IResponseProvider provider, int priority, object executionConditionState, object nextState)
        {
            Priority = priority;
            ExecutionConditionState = executionConditionState;
            NextState = nextState;
            Guid = guid;
            Title = title;
            RequestMatcher = requestMatcher;
            Provider = provider;
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
        /// Determines whether the RequestMessage is handled.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <returns>The <see cref="RequestMatchResult"/>.</returns>
        public RequestMatchResult IsRequestHandled(RequestMessage requestMessage)
        {
            var result = new RequestMatchResult();

            RequestMatcher.GetMatchingScore(requestMessage, result);

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