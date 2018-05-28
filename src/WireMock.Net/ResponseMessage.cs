using System.Collections.Generic;
using System.Linq;
using System.Text;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock
{
    /// <summary>
    /// The ResponseMessage.
    /// </summary>
    public class ResponseMessage
    {
        /// <summary>
        /// Gets the headers.
        /// </summary>
        public IDictionary<string, WireMockList<string>> Headers { get; set; } = new Dictionary<string, WireMockList<string>>();

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string BodyOriginal { get; set; }

        /// <summary>
        /// Gets or sets the body destination (SameAsSource, String or Bytes).
        /// </summary>
        public string BodyDestination { get; set; }

        /// <summary>
        /// Gets or sets the body as a string.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the body as a json object.
        /// </summary>
        public object BodyAsJson { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation and Newtonsoft.Json.JsonTextWriter.IndentChar settings.
        /// </summary>
        public bool? BodyAsJsonIndented { get; set; }

        /// <summary>
        /// Gets or sets the body as bytes.
        /// </summary>
        public byte[] BodyAsBytes { get; set; }

        /// <summary>
        /// Gets or sets the body as a file.
        /// </summary>
        public string BodyAsFile { get; set; }

        /// <summary>
        /// Is the body as file cached?
        /// </summary>
        public bool? BodyAsFileIsCached { get; set; }

        /// <summary>
        /// Gets or sets the body encoding.
        /// </summary>
        public Encoding BodyEncoding { get; set; } = new UTF8Encoding(false);

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddHeader(string name, string value)
        {
            Headers.Add(name, new WireMockList<string>(value));
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        public void AddHeader(string name, params string[] values)
        {
            Check.NotNullOrEmpty(values, nameof(values));

            var newHeaderValues = Headers.TryGetValue(name, out WireMockList<string> existingValues)
                ? values.Union(existingValues).ToArray()
                : values;

            Headers[name] = new WireMockList<string>(newHeaderValues);
        }
    }
}