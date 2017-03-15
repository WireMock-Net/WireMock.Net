using System;
using WireMock.Matchers.Request;

namespace WireMock.Logging
{
    /// <summary>
    /// LogEntry
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the request message.
        /// </summary>
        /// <value>
        /// The request message.
        /// </value>
        public RequestMessage RequestMessage { get; set; }

        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        /// <value>
        /// The response message.
        /// </value>
        public ResponseMessage ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the request match result.
        /// </summary>
        /// <value>
        /// The request match result.
        /// </value>
        public RequestMatchResult RequestMatchResult { get; set; }

        /// <summary>
        /// Gets or sets the mapping unique identifier.
        /// </summary>
        /// <value>
        /// The mapping unique identifier.
        /// </value>
        public Guid? MappingGuid { get; set; }

        /// <summary>
        /// Gets or sets the mapping unique title.
        /// </summary>
        /// <value>
        /// The mapping unique title.
        /// </value>
        public string MappingTitle { get; set; }
    }
}