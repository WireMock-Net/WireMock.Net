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
        /// <param name="provider">
        /// The provider.
        /// </param>
        void RespondWith(IResponseProvider provider);
    }
}