﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WireMock.Validation;

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
        public IDictionary<string, string[]> Headers { get; set; } = new ConcurrentDictionary<string, string[]>();

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
        /// Add header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddHeader(string name, string value)
        {
            Headers.Add(name, new[] { value });
        }

        /// <summary>
        /// Add header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="values">
        /// Header values.
        /// </param>
        public void AddHeader(string name, string[] values)
        {
            Check.NotEmpty(values, nameof(values));

            var newHeaderValues = Headers.TryGetValue(name, out string[] existingValues)
                ? values.Union(existingValues).ToArray()
                : values;

            Headers[name] = newHeaderValues;
        }
    }
}