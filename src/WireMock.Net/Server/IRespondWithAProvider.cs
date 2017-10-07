using System;

namespace WireMock.Server
{
    /// <summary>
    /// IRespondWithAProvider
    /// </summary>
    public interface IRespondWithAProvider
    {
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
        /// Execute this respond only in case the current state is equal to specified one.
        /// </summary>
        /// <param name="state">Any object which identifies the current state</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WhenStateIs(object state);

        /// <summary>
        /// Once this mapping is executed the state will be changed to specified one.
        /// </summary>
        /// <param name="state">Any object which identifies the new state</param>
        /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
        IRespondWithAProvider WillSetStateTo(object state);
    }
}