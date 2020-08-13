// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System.Collections.Generic;
using System.Linq;
using WireMock.ResponseBuilders;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock
{
    /// <summary>
    /// The ResponseMessage.
    /// </summary>
    public class ResponseMessage : IResponseMessage
    {
        /// <inheritdoc cref="IResponseMessage.Headers" />
        public IDictionary<string, WireMockList<string>> Headers { get; set; } = new Dictionary<string, WireMockList<string>>();

        /// <inheritdoc cref="IResponseMessage.StatusCode" />
        public object StatusCode { get; set; }

        /// <inheritdoc cref="IResponseMessage.BodyOriginal" />
        public string BodyOriginal { get; set; }

        /// <inheritdoc cref="IResponseMessage.BodyDestination" />
        public string BodyDestination { get; set; }

        /// <inheritdoc cref="IResponseMessage.BodyData" />
        public IBodyData BodyData { get; set; }

        /// <inheritdoc cref="IResponseMessage.FaultType" />
        public FaultType FaultType { get; set; }

        /// <inheritdoc cref="IResponseMessage.FaultPercentage" />
        public double? FaultPercentage { get; set; }

        /// <inheritdoc cref="IResponseMessage.AddHeader(string, string)" />
        public void AddHeader(string name, string value)
        {
            Headers.Add(name, new WireMockList<string>(value));
        }

        /// <inheritdoc cref="IResponseMessage.AddHeader(string, string[])" />
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