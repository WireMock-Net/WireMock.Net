using System;
using System.Text;
using JetBrains.Annotations;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The BodyResponseBuilder interface.
    /// </summary>
    public interface IBodyResponseBuilder : ITransformResponseBuilder
    {
        /// <summary>
        /// WithBody : Create a ... response based on a string.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="destination">The Body Destination format (SameAsSource, String or Bytes).</param>
        /// <param name="encoding">The body encoding.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithBody([NotNull] string body, [CanBeNull] string destination = BodyDestinationFormat.SameAsSource, [CanBeNull] Encoding encoding = null);

        /// <summary>
        /// WithBody : Create a ... response based on a callback function.
        /// </summary>
        /// <param name="bodyFactory">The delegate to build the body.</param>
        /// <param name="destination">The Body Destination format (SameAsSource, String or Bytes).</param>
        /// <param name="encoding">The body encoding.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithBody([NotNull] Func<RequestMessage, string> bodyFactory, [CanBeNull] string destination = BodyDestinationFormat.SameAsSource, [CanBeNull] Encoding encoding = null);

        /// <summary>
        /// WithBody : Create a ... response based on a bytearray.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="destination">The Body Destination format (SameAsSource, String or Bytes).</param>
        /// <param name="encoding">The body encoding.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithBody([NotNull] byte[] body, [CanBeNull] string destination = BodyDestinationFormat.SameAsSource, [CanBeNull] Encoding encoding = null);

        /// <summary>
        /// WithBody : Create a string response based on a object (which will be converted to a JSON string).
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="encoding">The body encoding.</param>
        /// <param name="indented">Use JSON indented.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithBodyAsJson([NotNull] object body, [CanBeNull] Encoding encoding = null, bool? indented = null);

        /// <summary>
        /// WithBody : Create a string response based on a object (which will be converted to a JSON string).
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="indented">Define whether child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation and Newtonsoft.Json.JsonTextWriter.IndentChar settings.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithBodyAsJson([NotNull] object body, bool indented);

        /// <summary>
        /// WithBody : Create a string response based on a Base64 string (which will be decoded to a normal string).
        /// </summary>
        /// <param name="bodyAsbase64">The body.</param>
        /// <param name="encoding">The Encoding.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        [Obsolete("Should not be used, will be removed in future.")]
        IResponseBuilder WithBodyFromBase64([NotNull] string bodyAsbase64, [CanBeNull] Encoding encoding = null);

        /// <summary>
        /// WithBodyFromFile : Create a ... response based on a File.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="cache">Defines if this file is cached in memory or retrieved from disk everytime the response is created.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithBodyFromFile([NotNull] string filename, bool cache = true);
    }
}