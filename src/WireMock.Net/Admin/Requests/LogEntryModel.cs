using System;

namespace WireMock.Admin.Requests
{
    /// <summary>
    /// Request Log Model
    /// </summary>
    public class LogEntryModel
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        public LogRequestModel Request { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public LogResponseModel Response { get; set; }

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

        /// <summary>
        /// Gets or sets the request match result.
        /// </summary>
        /// <value>
        /// The request match result.
        /// </value>
        public LogRequestMatchModel RequestMatchResult { get; set; }
    }
}