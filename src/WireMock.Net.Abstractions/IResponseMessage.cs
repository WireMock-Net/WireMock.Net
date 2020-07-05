using System.Collections.Generic;
using WireMock.ResponseBuilders;
using WireMock.Types;
using WireMock.Util;

namespace WireMock
{
    /// <summary>
    /// IResponseMessage
    /// </summary>
    public interface IResponseMessage
    {
        /// <summary>
        /// The Body.
        /// </summary>
        IBodyData BodyData { get; }

        /// <summary>
        /// Gets the body destination (SameAsSource, String or Bytes).
        /// </summary>
        string BodyDestination { get; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        string BodyOriginal { get; }

        /// <summary>
        /// Gets the Fault percentage.
        /// </summary>
        double? FaultPercentage { get; }

        /// <summary>
        /// The FaultType.
        /// </summary>
        FaultType FaultType { get; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        IDictionary<string, WireMockList<string>> Headers { get; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        object StatusCode { get; }

        //void AddHeader(string name, params string[] values);
        //void AddHeader(string name, string value);
    }
}