using WireMock.Matchers.Request;

namespace WireMock.Server
{
    /// <summary>
    /// The respond with a provider.
    /// </summary>
    internal class RespondWithAProvider : IRespondWithAProvider
    {
        /// <summary>
        /// The _registration callback.
        /// </summary>
        private readonly RegistrationCallback _registrationCallback;

        /// <summary>
        /// The _request matcher.
        /// </summary>
        private readonly IRequestMatcher _requestMatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="RespondWithAProvider"/> class.
        /// </summary>
        /// <param name="registrationCallback">The registration callback.</param>
        /// <param name="requestMatcher">The request matcher.</param>
        public RespondWithAProvider(RegistrationCallback registrationCallback, IRequestMatcher requestMatcher)
        {
            _registrationCallback = registrationCallback;
            _requestMatcher = requestMatcher;
        }

        /// <summary>
        /// The respond with.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public void RespondWith(IResponseProvider provider)
        {
            _registrationCallback(new Mapping(_requestMatcher, provider));
        }
    }
}