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
        /// Scenario.
        /// </summary>
        public string Scenario { get; set; }

        /// <summary>
        /// Execution state condition for the current mapping.
        /// </summary>
        public object WhenStateIs { get; set; }

        /// <summary>
        /// The next state which will be signaled after the current mapping execution.
        /// In case the value is null state will not be changed.
        /// </summary>
        public object SetStateTo { get; set; }

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