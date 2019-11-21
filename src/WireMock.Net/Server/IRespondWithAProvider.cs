using System;
using WireMock.ResponseProviders;

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
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WillSetStateTo(string state);

        /// <summary>
        /// Once this mapping is executed the state will be changed to specified one.
        /// </summary>
        /// <param name="state">Any object which identifies the new state</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WillSetStateTo(int state);
    }
}