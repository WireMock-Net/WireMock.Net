using System;

namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// MappingModel
    /// </summary>
    public class MappingModel
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid? Guid { get; set; }

        /// <summary>
        /// Gets or sets the unique title.
        /// </summary>
        /// <value>
        /// The unique title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int? Priority { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        public RequestModel Request { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public ResponseModel Response { get; set; }
    }
}