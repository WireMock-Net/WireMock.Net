using System;

namespace WireMock.Server
{
    /// <summary>
    /// IRespondWithAProviderGuid
    /// </summary>
    public interface IRespondWithAProviderGuid : IRespondWithAProvider
    {
        /// <summary>
        /// Define a unique identifier for this mapping.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>The <see cref="IRespondWithAProviderGuid"/>.</returns>
        IRespondWithAProviderGuid WithGuid(Guid guid);
    }
}