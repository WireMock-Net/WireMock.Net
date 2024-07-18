// Copyright © WireMock.Net

using System;
using System.Text;
using System.Threading.Tasks;
using JsonConverter.Abstractions;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The BodyResponseBuilder interface.
/// </summary>
public interface IBodyResponseBuilder : IFaultResponseBuilder
{
    /// <summary>
    /// WithBody : Create a ... response based on a string.
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="destination">The Body Destination format (SameAsSource, String or Bytes).</param>
    /// <param name="encoding">The body encoding.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBody(string body, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null);

    /// <summary>
    /// WithBody : Create a ... response based on a callback function.
    /// </summary>
    /// <param name="bodyFactory">The delegate to build the body.</param>
    /// <param name="destination">The Body Destination format (SameAsSource, String or Bytes).</param>
    /// <param name="encoding">The body encoding.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBody(Func<IRequestMessage, string> bodyFactory, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null);

    /// <summary>
    /// WithBody : Create a ... response based on a async callback function.
    /// </summary>
    /// <param name="bodyFactory">The async delegate to build the body.</param>
    /// <param name="destination">The Body Destination format (SameAsSource, String or Bytes).</param>
    /// <param name="encoding">The body encoding.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBody(Func<IRequestMessage, Task<string>> bodyFactory, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null);

    /// <summary>
    /// WithBody : Create a ... response based on a bytearray.
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="destination">The Body Destination format (SameAsSource, String or Bytes).</param>
    /// <param name="encoding">The body encoding.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBody(byte[] body, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null);

    /// <summary>
    /// WithBody : Create a string response based on a object (which will be converted to a JSON string).
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="encoding">The body encoding.</param>
    /// <param name="indented">Define whether child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation and Newtonsoft.Json.JsonTextWriter.IndentChar settings.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBodyAsJson(object body, Encoding? encoding = null, bool? indented = null);

    /// <summary>
    /// WithBody : Create a string response based on a object (which will be converted to a JSON string).
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="indented">Define whether child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation and Newtonsoft.Json.JsonTextWriter.IndentChar settings.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBodyAsJson(object body, bool indented);

    /// <summary>
    /// WithBodyAsJson : Create a ... response based on a callback function.
    /// </summary>
    /// <param name="bodyFactory">The delegate to build the body.</param>
    /// <param name="encoding">The body encoding.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBodyAsJson(Func<IRequestMessage, object> bodyFactory, Encoding? encoding = null);

    /// <summary>
    /// WithBodyAsJson : Create a ... response based on a async callback function.
    /// </summary>
    /// <param name="bodyFactory">The async delegate to build the body.</param>
    /// <param name="encoding">The body encoding.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBodyAsJson(Func<IRequestMessage, Task<object>> bodyFactory, Encoding? encoding = null);

    /// <summary>
    /// WithBodyFromFile : Create a ... response based on a File.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="cache">Defines if this file is cached in memory or retrieved from disk every time the response is created.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBodyFromFile(string filename, bool cache = true);

    /// <summary>
    /// WithBody : Create a string response based on a object (which will be converted to a JSON string using the <see cref="IJsonConverter"/>).
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="jsonConverter">The <see cref="IJsonConverter"/>.</param>
    /// <param name="options">The <see cref="JsonConverterOptions"/> [optional].</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBody(object body, IJsonConverter jsonConverter, JsonConverterOptions? options = null);

    /// <summary>
    /// WithBody : Create a string response based on a object (which will be converted to a JSON string using the <see cref="IJsonConverter"/>).
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="encoding">The body encoding, can be <c>null</c>.</param>
    /// <param name="jsonConverter">The <see cref="IJsonConverter"/>.</param>
    /// <param name="options">The <see cref="JsonConverterOptions"/> [optional].</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBody(object body, Encoding? encoding, IJsonConverter jsonConverter, JsonConverterOptions? options = null);

    /// <summary>
    /// WithBody : Create a ProtoBuf byte[] response based on a proto definition, message type and the value.
    /// </summary>
    /// <param name="protoDefinition">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="value">The object to convert to protobuf byte[].</param>
    /// <param name="jsonConverter">The <see cref="IJsonConverter"/> [optional]. Default value is NewtonsoftJsonConverter.</param>
    /// <param name="options">The <see cref="JsonConverterOptions"/> [optional].</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBodyAsProtoBuf(
        string protoDefinition,
        string messageType,
        object value,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? options = null
    );

    /// <summary>
    /// WithBody : Create a ProtoBuf byte[] response based on a proto definition, message type and the value.
    /// </summary>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="value">The object to convert to protobuf byte[].</param>
    /// <param name="jsonConverter">The <see cref="IJsonConverter"/> [optional]. Default value is NewtonsoftJsonConverter.</param>
    /// <param name="options">The <see cref="JsonConverterOptions"/> [optional].</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithBodyAsProtoBuf(
        string messageType,
        object value,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? options = null
    );
}