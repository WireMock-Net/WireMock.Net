using System;
using WireMock.Matchers.Request;

namespace WireMock.Logging
{
    public interface ILogEntry
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        Guid Guid { get; }

        /// <summary>
        /// Gets the mapping unique identifier.
        /// </summary>
        /// <value>
        /// The mapping unique identifier.
        /// </value>
        Guid? MappingGuid { get; }

        /// <summary>
        /// Gets the mapping unique title.
        /// </summary>
        /// <value>
        /// The mapping unique title.
        /// </value>
        string MappingTitle { get; }

        /// <summary>
        /// Gets the partial mapping unique identifier.
        /// </summary>
        /// <value>
        /// The mapping unique identifier.
        /// </value>
        Guid? PartialMappingGuid { get; }

        /// <summary>
        /// Gets the partial mapping unique title.
        /// </summary>
        /// <value>
        /// The mapping unique title.
        /// </value>
        string PartialMappingTitle { get; }

        /// <summary>
        /// Gets the partial match result.
        /// </summary>
        /// <value>
        /// The request match result.
        /// </value>
        IRequestMatchResult PartialMatchResult { get; }

        /// <summary>
        /// Gets the request match result.
        /// </summary>
        /// <value>
        /// The request match result.
        /// </value>
        IRequestMatchResult RequestMatchResult { get; }

        /// <summary>
        /// Gets the request message.
        /// </summary>
        /// <value>
        /// The request message.
        /// </value>
        IRequestMessage RequestMessage { get; }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <value>
        /// The response message.
        /// </value>
        IResponseMessage ResponseMessage { get; }
    }
}