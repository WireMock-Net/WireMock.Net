// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.ResponseProviders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

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
        private int _timesInSameState = 1;
        private readonly RegistrationCallback _registrationCallback;
        private readonly IRequestMatcher _requestMatcher;
        private readonly IWireMockServerSettings _settings;
        private readonly bool _saveToFile;

        public Guid Guid { get; private set; } = Guid.NewGuid();

        public IWebhook[] Webhooks { get; private set; }

        public ITimeSettings TimeSettings { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RespondWithAProvider"/> class.
        /// </summary>
        /// <param name="registrationCallback">The registration callback.</param>
        /// <param name="requestMatcher">The request matcher.</param>
        /// <param name="settings">The WireMockServerSettings.</param>
        /// <param name="saveToFile">Optional boolean to indicate if this mapping should be saved as static mapping file.</param>
        public RespondWithAProvider(RegistrationCallback registrationCallback, IRequestMatcher requestMatcher, IWireMockServerSettings settings, bool saveToFile = false)
        {
            _registrationCallback = registrationCallback;
            _requestMatcher = requestMatcher;
            _settings = settings;
            _saveToFile = saveToFile;
        }

        /// <summary>
        /// The respond with.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void RespondWith(IResponseProvider provider)
        {
            _registrationCallback(new Mapping(Guid, _title, _path, _settings, _requestMatcher, provider, _priority, _scenario, _executionConditionState, _nextState, _timesInSameState, Webhooks, TimeSettings), _saveToFile);
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

        /// <see cref="IRespondWithAProvider.InScenario(int)"/>
        public IRespondWithAProvider InScenario(int scenario)
        {
            return InScenario(scenario.ToString());
        }

        /// <see cref="IRespondWithAProvider.WhenStateIs(string)"/>
        public IRespondWithAProvider WhenStateIs(string state)
        {
            if (string.IsNullOrEmpty(_scenario))
            {
                throw new NotSupportedException("Unable to set state condition when no scenario is defined.");
            }

            _executionConditionState = state;

            return this;
        }

        /// <see cref="IRespondWithAProvider.WhenStateIs(int)"/>
        public IRespondWithAProvider WhenStateIs(int state)
        {
            return WhenStateIs(state.ToString());
        }

        /// <see cref="IRespondWithAProvider.WillSetStateTo(string, int?)"/>
        public IRespondWithAProvider WillSetStateTo(string state, int? times = 1)
        {
            if (string.IsNullOrEmpty(_scenario))
            {
                throw new NotSupportedException("Unable to set next state when no scenario is defined.");
            }

            _nextState = state;
            _timesInSameState = times ?? 1;

            return this;
        }

        /// <see cref="IRespondWithAProvider.WillSetStateTo(int, int?)"/>
        public IRespondWithAProvider WillSetStateTo(int state, int? times = 1)
        {
            return WillSetStateTo(state.ToString(), times);
        }

        /// <inheritdoc />
        public IRespondWithAProvider WithTimeSettings(ITimeSettings timeSettings)
        {
            Check.NotNull(timeSettings, nameof(timeSettings));

            TimeSettings = timeSettings;

            return this;
        }

        /// <see cref="IRespondWithAProvider.WithWebhook(IWebhook[])"/>
        public IRespondWithAProvider WithWebhook(params IWebhook[] webhooks)
        {
            Check.HasNoNulls(webhooks, nameof(webhooks));

            Webhooks = webhooks;

            return this;
        }

        /// <see cref="IRespondWithAProvider.WithWebhook(string, string, IDictionary{string, WireMockList{string}}, string, bool, TransformerType)"/>
        public IRespondWithAProvider WithWebhook(
            [NotNull] string url,
            [CanBeNull] string method = "post",
            [CanBeNull] IDictionary<string, WireMockList<string>> headers = null,
            [CanBeNull] string body = null,
            bool useTransformer = true,
            TransformerType transformerType = TransformerType.Handlebars)
        {
            Webhooks = new[] { InitWebhook(url, method, headers, useTransformer, transformerType) };

            if (body != null)
            {
                Webhooks[0].Request.BodyData = new BodyData
                {
                    BodyAsString = body,
                    DetectedBodyType = BodyType.String,
                    DetectedBodyTypeFromContentType = BodyType.String
                };
            }

            return this;
        }

        /// <see cref="IRespondWithAProvider.WithWebhook(string, string, IDictionary{string, WireMockList{string}}, object, bool, TransformerType)"/>
        public IRespondWithAProvider WithWebhook(
            [NotNull] string url,
            [CanBeNull] string method = "post",
            [CanBeNull] IDictionary<string, WireMockList<string>> headers = null,
            [CanBeNull] object body = null,
            bool useTransformer = true,
            TransformerType transformerType = TransformerType.Handlebars)
        {
            Webhooks = new[] { InitWebhook(url, method, headers, useTransformer, transformerType) };

            if (body != null)
            {
                Webhooks[0].Request.BodyData = new BodyData
                {
                    BodyAsJson = body,
                    DetectedBodyType = BodyType.Json,
                    DetectedBodyTypeFromContentType = BodyType.Json
                };
            }

            return this;
        }

        private static IWebhook InitWebhook(
            string url,
            string method,
            IDictionary<string, WireMockList<string>> headers,
            bool useTransformer,
            TransformerType transformerType)
        {
            return new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = url,
                    Method = method ?? "post",
                    Headers = headers,
                    UseTransformer = useTransformer,
                    TransformerType = transformerType
                }
            };
        }
    }
}