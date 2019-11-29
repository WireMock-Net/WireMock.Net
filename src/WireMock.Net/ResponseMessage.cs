using System.Collections.Generic;
using System.Linq;
using WireMock.ResponseBuilders;
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
        public object StatusCode { get; set; } = 200;

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string BodyOriginal { get; set; }

        /// <summary>
        /// Gets or sets the body destination (SameAsSource, String or Bytes).
        /// </summary>
        public string BodyDestination { get; set; }

        /// <summary>
        /// The Body.
        /// </summary>
        public BodyData BodyData { get; set; }

        /// <summary>
        /// The FaultType.
        /// </summary>
        public FaultType FaultType { get; set; }

        /// <summary>
        /// Gets or sets the Fault percentage.
        /// </summary>
        public double? FaultPercentage { get; set; }

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