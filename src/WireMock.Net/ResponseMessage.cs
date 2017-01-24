using System.Collections.Concurrent;
using System.Collections.Generic;
namespace WireMock
{
    /// <summary>
    /// The response.
    /// </summary>
    public class ResponseMessage
    {
        /// <summary>
        /// Gets the headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The add header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddHeader(string name, string value)
        {
            Headers.Add(name, value);
        }
    }
}