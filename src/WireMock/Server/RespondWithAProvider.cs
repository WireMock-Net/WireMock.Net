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
        private readonly IRequestMatcher _requestSpec;

        /// <summary>
        /// Initializes a new instance of the <see cref="RespondWithAProvider"/> class.
        /// </summary>
        /// <param name="registrationCallback">
        /// The registration callback.
        /// </param>
        /// <param name="requestSpec">
        /// The request matcher.
        /// </param>
        public RespondWithAProvider(RegistrationCallback registrationCallback, IRequestMatcher requestSpec)
        {
            _registrationCallback = registrationCallback;
            _requestSpec = requestSpec;
        }

        /// <summary>
        /// The respond with.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public void RespondWith(IProvideResponses provider)
        {
            _registrationCallback(new Route(_requestSpec, provider));
        }
    }
}