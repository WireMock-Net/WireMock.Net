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
        private string _title;
        private object _executionConditionState;
        private object _nextState;
        private string _scenario;

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
            _registrationCallback(new Mapping(mappingGuid, _title, _requestMatcher, provider, _priority, _scenario, _executionConditionState, _nextState));
        }

        /// <summary>
        /// Define a unique identifier for this mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        public IRespondWithAProvider WithGuid(string guid)
        {
            return WithGuid(Guid.Parse(guid));
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
        /// Define a unique identifier for this mapping.
        /// </summary>
        /// <param name="title">The unique identifier.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        public IRespondWithAProvider WithTitle(string title)
        {
            _title = title;

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

        public IRespondWithAProvider InScenario(string scenario)
        {
            _scenario = scenario;

            return this;
        }

        public IRespondWithAProvider WhenStateIs(object state)
        {
            if (string.IsNullOrEmpty(_scenario))
            {
                throw new NotSupportedException("Unable to set state condition when no scenario is defined.");
            }

            //if (_nextState != null)
            //{
            //    throw new NotSupportedException("Unable to set state condition when next state is defined.");
            //}

            _executionConditionState = state;

            return this;
        }

        public IRespondWithAProvider WillSetStateTo(object state)
        {
            if (string.IsNullOrEmpty(_scenario))
            {
                throw new NotSupportedException("Unable to set next state when no scenario is defined.");
            }

            _nextState = state;

            return this;
        }
    }
}