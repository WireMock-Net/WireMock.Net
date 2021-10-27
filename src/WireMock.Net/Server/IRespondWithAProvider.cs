using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Models;
using WireMock.ResponseProviders;
using WireMock.Types;

namespace WireMock.Server
{
    /// <summary>
    /// IRespondWithAProvider
    /// </summary>
    public interface IRespondWithAProvider
    {
        /// <summary>
        /// Gets the unique identifier for this mapping.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Define a unique identifier for this mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WithGuid(Guid guid);

        /// <summary>
        /// Define the TimeSettings for this mapping.
        /// </summary>
        /// <param name="timeSettings">The TimeSettings.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WithTimeSettings(ITimeSettings timeSettings);

        /// <summary>
        /// Define a unique title for this mapping.
        /// </summary>
        /// <param name="title">The unique title.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WithTitle(string title);

        /// <summary>
        /// Define the full filepath for this mapping.
        /// </summary>
        /// <param name="path">The full filepath.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WithPath(string path);

        /// <summary>
        /// Define a unique identifier for this mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WithGuid(string guid);

        /// <summary>
        /// Define the priority for this mapping.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider AtPriority(int priority);

        /// <summary>
        /// The respond with.
        /// </summary>
        /// <param name="provider">The provider.</param>
        void RespondWith(IResponseProvider provider);

        /// <summary>
        /// Sets the the scenario.
        /// </summary>
        /// <param name="scenario">The scenario.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider InScenario(string scenario);

        /// <summary>
        /// Sets the the scenario with an integer value.
        /// </summary>
        /// <param name="scenario">The scenario.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider InScenario(int scenario);

        /// <summary>
        /// Execute this respond only in case the current state is equal to specified one.
        /// </summary>
        /// <param name="state">Any object which identifies the current state</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WhenStateIs(string state);

        /// <summary>
        /// Execute this respond only in case the current state is equal to specified one.
        /// </summary>
        /// <param name="state">Any object which identifies the current state</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WhenStateIs(int state);

        /// <summary>
        /// Once this mapping is executed the state will be changed to specified one.
        /// </summary>
        /// <param name="state">Any object which identifies the new state</param>
        /// <param name="times">The number of times this match should be matched before the state will be changed to the specified one. Default value is 1.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WillSetStateTo(string state, int? times = 1);

        /// <summary>
        /// Once this mapping is executed the state will be changed to specified one.
        /// </summary>
        /// <param name="state">Any object which identifies the new state</param>
        /// <param name="times">The number of times this match should be matched before the state will be changed to the specified one. Default value is 1.</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WillSetStateTo(int state, int? times = 1);

        /// <summary>
        /// Add (multiple) Webhook(s) to call after the response has been generated.
        /// </summary>
        /// <param name="webhooks">The Webhooks</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WithWebhook(params IWebhook[] webhooks);

        /// <summary>
        /// Add a Webhook to call after the response has been generated.
        /// </summary>
        /// <param name="url">The Webhook Url</param>
        /// <param name="method">The method to use. [optional]</param>
        /// <param name="headers">The Headers to send. [optional]</param>
        /// <param name="body">The body (as string) to send. [optional]</param>
        /// <param name="useTransformer">Use Transformer. [optional]</param>
        /// <param name="transformerType">The transformer type. [optional]</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WithWebhook(
            [NotNull] string url,
            [CanBeNull] string method = "post",
            [CanBeNull] IDictionary<string, WireMockList<string>> headers = null,
            [CanBeNull] string body = null,
            bool useTransformer = true,
            TransformerType transformerType = TransformerType.Handlebars
        );

        /// <summary>
        /// Add a Webhook to call after the response has been generated.
        /// </summary>
        /// <param name="url">The Webhook Url</param>
        /// <param name="method">The method to use. [optional]</param>
        /// <param name="headers">The Headers to send. [optional]</param>
        /// <param name="body">The body (as json) to send. [optional]</param>
        /// <param name="useTransformer">Use Transformer. [optional]</param>
        /// <param name="transformerType">The transformer type. [optional]</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WithWebhook(
            [NotNull] string url,
            [CanBeNull] string method = "post",
            [CanBeNull] IDictionary<string, WireMockList<string>> headers = null,
            [CanBeNull] object body = null,
            bool useTransformer = true,
            TransformerType transformerType = TransformerType.Handlebars
        );
    }
}