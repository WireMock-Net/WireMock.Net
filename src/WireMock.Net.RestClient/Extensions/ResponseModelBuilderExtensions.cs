// Copyright © WireMock.Net

using System.Text;
using JsonConverter.Abstractions;
using JsonConverter.Newtonsoft.Json;
using WireMock.Admin.Mappings;

namespace WireMock.Client.Extensions;

/// <summary>
/// ResponseModelBuilder
/// </summary>
public static class ResponseModelBuilderExtensions
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
    private static readonly IJsonConverter JsonConverter = new NewtonsoftJsonConverter();

    /// <summary>
    /// WithBodyAsJson
    /// </summary>
    /// <param name="builder">The ResponseModelBuilder.</param>
    /// <param name="body">The body.</param>
    /// <param name="encoding">The body encoding.</param>
    /// <param name="indented">Define whether child objects to be indented.</param>
    public static ResponseModelBuilder WithBodyAsJson(this ResponseModelBuilder builder, object body, Encoding? encoding = null, bool? indented = null)
    {
        return builder.WithBodyAsBytes(() =>
        {
            var options = new JsonConverterOptions
            {
                WriteIndented = indented == true
            };
            var jsonBody = JsonConverter.Serialize(body, options);
            return (encoding ?? Utf8NoBom).GetBytes(jsonBody);
        });
    }

    /// <summary>
    /// WithBodyAsJson
    /// </summary>
    /// <param name="builder">The ResponseModelBuilder.</param>
    /// <param name="body">The body.</param>
    /// <param name="indented">Define whether child objects to be indented.</param>
    public static ResponseModelBuilder WithBodyAsJson(this ResponseModelBuilder builder, object body, bool indented)
    {
        return builder.WithBodyAsJson(body, null, indented);
    }
}