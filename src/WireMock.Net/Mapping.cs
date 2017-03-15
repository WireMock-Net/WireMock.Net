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
        {
            Priority = priority;
            Guid = guid;
            Title = title;
            RequestMatcher = requestMatcher;
            Provider = provider;
        }

        /// <summary>
        /// The response to.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResponseMessage> ResponseTo(RequestMessage requestMessage)
        {
            return await Provider.ProvideResponse(requestMessage);
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
        public bool IsAdminInterface => Provider is DynamicResponseProvider;
    }
}