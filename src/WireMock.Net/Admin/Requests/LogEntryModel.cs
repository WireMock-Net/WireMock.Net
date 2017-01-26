namespace WireMock.Admin.Requests
{
    /// <summary>
    /// Request Log Model
    /// </summary>
    public class LogEntryModel
    {
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
    }
}