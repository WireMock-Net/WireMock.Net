using System;
using WireMock.Matchers.Request;

namespace WireMock.Server
{
    /// <summary>
    /// The respond with a provider.
    /// </summary>
    internal class RespondWithAProvider : IRespondWithAProvider
    {
        private int _priority;
        private Guid? _guid;

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
            var mappingGuid = _guid ?? Guid.NewGuid();
            _registrationCallback(new Mapping(mappingGuid, _requestMatcher, provider, _priority));
        }

        /// <summary>
        /// Define a unique identifier for this mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        public IRespondWithAProvider WithGuid(Guid guid)
        {
            _guid = guid;

            return this;
        }

        /// <summary>
        /// Define the priority for this mapping.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        public IRespondWithAProvider AtPriority(int priority)
        {
            _priority = priority;

            return this;
        }
    }
}