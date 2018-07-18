using System;
using WireMock.Matchers.Request;
using WireMock.ResponseProviders;

namespace WireMock.Server
{
    /// <summary>
    /// The respond with a provider.
    /// </summary>
    internal class RespondWithAProvider : IRespondWithAProvider
    {
        private int _priority;
        private string _title;
        private string _path;
        private string _executionConditionState;
        private string _nextState;
        private string _scenario;
        private readonly RegistrationCallback _registrationCallback;
        private readonly IRequestMatcher _requestMatcher;

        public Guid Guid { get; private set; } = Guid.NewGuid();

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
        /// <param name="provider">The provider.</param>
        public void RespondWith(IResponseProvider provider)
        {
            _registrationCallback(new Mapping(Guid, _title, _path, _requestMatcher, provider, _priority, _scenario, _executionConditionState, _nextState));
        }

        /// <see cref="IRespondWithAProvider.WithGuid(string)"/>
        public IRespondWithAProvider WithGuid(string guid)
        {
            return WithGuid(Guid.Parse(guid));
        }

        /// <see cref="IRespondWithAProvider.WithGuid(Guid)"/>
        public IRespondWithAProvider WithGuid(Guid guid)
        {
            Guid = guid;

            return this;
        }

        /// <see cref="IRespondWithAProvider.WithTitle"/>
        public IRespondWithAProvider WithTitle(string title)
        {
            _title = title;

            return this;
        }

        /// <see cref="IRespondWithAProvider.WithPath"/>
        public IRespondWithAProvider WithPath(string path)
        {
            _path = path;

            return this;
        }

        /// <see cref="IRespondWithAProvider.AtPriority"/>
        public IRespondWithAProvider AtPriority(int priority)
        {
            _priority = priority;

            return this;
        }

        /// <see cref="IRespondWithAProvider.InScenario(string)"/>
        public IRespondWithAProvider InScenario(string scenario)
        {
            _scenario = scenario;

            return this;
        }

        /// <see cref="IRespondWithAProvider.WhenStateIs"/>
        public IRespondWithAProvider WhenStateIs(string state)
        {
            if (string.IsNullOrEmpty(_scenario))
            {
                throw new NotSupportedException("Unable to set state condition when no scenario is defined.");
            }

            _executionConditionState = state;

            return this;
        }

        /// <see cref="IRespondWithAProvider.WillSetStateTo"/>
        public IRespondWithAProvider WillSetStateTo(string state)
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