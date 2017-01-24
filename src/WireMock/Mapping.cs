using System.Threading.Tasks;
using WireMock.Matchers.Request;

namespace WireMock
{
    /// <summary>
    /// The route.
    /// </summary>
    public class Mapping
    {
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
        /// <param name="requestMatcher">The request matcher.</param>
        /// <param name="provider">The provider.</param>
        public Mapping(IRequestMatcher requestMatcher, IResponseProvider provider)
        {
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
        /// <returns>
        ///   <c>true</c> if RequestMessage is handled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestHandled(RequestMessage requestMessage)
        {
            return RequestMatcher.IsMatch(requestMessage);
        }
    }
}