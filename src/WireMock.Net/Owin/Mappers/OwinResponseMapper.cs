using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using WireMock.Http;
using WireMock.ResponseBuilders;
using WireMock.Types;
using Stef.Validation;
#if !USE_ASPNETCORE
using IResponse = Microsoft.Owin.IOwinResponse;
#else
using Microsoft.AspNetCore.Http;
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
#endif

namespace WireMock.Owin.Mappers
{
    /// <summary>
    /// OwinResponseMapper
    /// </summary>
    internal class OwinResponseMapper : IOwinResponseMapper
    {
        private readonly IRandomizerNumber<double> _randomizerDouble = RandomizerFactory.GetRandomizer(new FieldOptionsDouble { Min = 0, Max = 1 });
        private readonly IRandomizerBytes _randomizerBytes = RandomizerFactory.GetRandomizer(new FieldOptionsBytes { Min = 100, Max = 200 });
        private readonly IWireMockMiddlewareOptions _options;
        private readonly Encoding _utf8NoBom = new UTF8Encoding(false);

        // https://msdn.microsoft.com/en-us/library/78h415ay(v=vs.110).aspx
#if !USE_ASPNETCORE
        private static readonly IDictionary<string, Action<IResponse, WireMockList<string>>> ResponseHeadersToFix = new Dictionary<string, Action<IResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#else
        private static readonly IDictionary<string, Action<IResponse, WireMockList<string>>> ResponseHeadersToFix = new Dictionary<string, Action<IResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#endif
            { HttpKnownHeaderNames.ContentType, (r, v) => r.ContentType = v.FirstOrDefault() }
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">The IWireMockMiddlewareOptions.</param>
        public OwinResponseMapper(IWireMockMiddlewareOptions options)
        {
            _options = Guard.NotNull(options);
        }

        /// <inheritdoc cref="IOwinResponseMapper.MapAsync"/>
        public async Task MapAsync(IResponseMessage? responseMessage, IResponse response)
        {
            if (responseMessage == null)
            {
                return;
            }

            byte[]? bytes;
            switch (responseMessage.FaultType)
            {
                case FaultType.EMPTY_RESPONSE:
                    bytes = IsFault(responseMessage) ? EmptyArray<byte>.Value : GetNormalBody(responseMessage);
                    break;

                case FaultType.MALFORMED_RESPONSE_CHUNK:
                    bytes = GetNormalBody(responseMessage) ?? EmptyArray<byte>.Value;
                    if (IsFault(responseMessage))
                    {
                        bytes = bytes.Take(bytes.Length / 2).Union(_randomizerBytes.Generate()).ToArray();
                    }
                    break;

                default:
                    bytes = GetNormalBody(responseMessage);
                    break;
            }

            var statusCodeType = responseMessage.StatusCode?.GetType();
            switch (statusCodeType)
            {
                case { } typeAsIntOrEnum when typeAsIntOrEnum == typeof(int) || typeAsIntOrEnum == typeof(int?) || typeAsIntOrEnum.GetTypeInfo().IsEnum:
                    response.StatusCode = MapStatusCode((int)responseMessage.StatusCode!);
                    break;

                case { } typeAsString when typeAsString == typeof(string):
                    // Note: this case will also match on null 
                    int.TryParse(responseMessage.StatusCode as string, out var result);
                    response.StatusCode = MapStatusCode(result);
                    break;

                default:
                    break;
            }

            SetResponseHeaders(responseMessage, response);

            if (bytes != null)
            {
                try
                {
                    await response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _options.Logger.Warn("Error writing response body. Exception : {0}", ex);
                }
            }
        }

        private int MapStatusCode(int code)
        {
            if (_options.AllowOnlyDefinedHttpStatusCodeInResponse == true && !Enum.IsDefined(typeof(HttpStatusCode), code))
            {
                return (int)HttpStatusCode.OK;
            }

            return code;
        }

        private bool IsFault(IResponseMessage responseMessage)
        {
            return responseMessage.FaultPercentage == null || _randomizerDouble.Generate() <= responseMessage.FaultPercentage;
        }

        private byte[]? GetNormalBody(IResponseMessage responseMessage)
        {
            switch (responseMessage.BodyData?.DetectedBodyType)
            {
                case BodyType.String:
                case BodyType.FormUrlEncoded:
                    return (responseMessage.BodyData.Encoding ?? _utf8NoBom).GetBytes(responseMessage.BodyData.BodyAsString!);

                case BodyType.Json:
                    var formatting = responseMessage.BodyData.BodyAsJsonIndented == true
                        ? Formatting.Indented
                        : Formatting.None;
                    string jsonBody = JsonConvert.SerializeObject(responseMessage.BodyData.BodyAsJson, new JsonSerializerSettings { Formatting = formatting, NullValueHandling = NullValueHandling.Ignore });
                    return (responseMessage.BodyData.Encoding ?? _utf8NoBom).GetBytes(jsonBody);

                case BodyType.Bytes:
                    return responseMessage.BodyData.BodyAsBytes;

                case BodyType.File:
                    return _options.FileSystemHandler?.ReadResponseBodyAsFile(responseMessage.BodyData.BodyAsFile!);
            }

            return null;
        }

        private static void SetResponseHeaders(IResponseMessage responseMessage, IResponse response)
        {
            // Force setting the Date header (#577)
            AppendResponseHeader(
                response,
                HttpKnownHeaderNames.Date,
                new[]
                {
                    DateTime.UtcNow.ToString(CultureInfo.InvariantCulture.DateTimeFormat.RFC1123Pattern, CultureInfo.InvariantCulture)
                });

            // Set other headers
            foreach (var item in responseMessage.Headers!)
            {
                var headerName = item.Key;
                var value = item.Value;
                if (ResponseHeadersToFix.ContainsKey(headerName))
                {
                    ResponseHeadersToFix[headerName]?.Invoke(response, value);
                }
                else
                {
                    // Check if this response header can be added (#148 and #227)
                    if (!HttpKnownHeaderNames.IsRestrictedResponseHeader(headerName))
                    {
                        AppendResponseHeader(response, headerName, value.ToArray());
                    }
                }
            }
        }

        private static void AppendResponseHeader(IResponse response, string headerName, string[] values)
        {
#if !USE_ASPNETCORE
            response.Headers.AppendValues(headerName, values);
#else
            response.Headers.Append(headerName, values);
#endif
        }
    }
}