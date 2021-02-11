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
using WireMock.Validation;
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
            Check.NotNull(options, nameof(options));

            _options = options;
        }

        /// <inheritdoc cref="IOwinResponseMapper.MapAsync"/>
        public async Task MapAsync(ResponseMessage responseMessage, IResponse response)
        {
            if (responseMessage == null)
            {
                return;
            }

            byte[] bytes;
            switch (responseMessage.FaultType)
            {
                case FaultType.EMPTY_RESPONSE:
                    bytes = IsFault(responseMessage) ? new byte[0] : GetNormalBody(responseMessage);
                    break;

                case FaultType.MALFORMED_RESPONSE_CHUNK:
                    bytes = GetNormalBody(responseMessage) ?? new byte[0];
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
                case Type typeAsIntOrEnum when typeAsIntOrEnum == typeof(int) || typeAsIntOrEnum == typeof(int?) || typeAsIntOrEnum.GetTypeInfo().IsEnum:
                    response.StatusCode = MapStatusCode((int)responseMessage.StatusCode);
                    break;

                case Type typeAsString when typeAsString == typeof(string):
                    // Note: this case will also match on null 
                    int.TryParse(responseMessage.StatusCode as string, out int result);
                    response.StatusCode = MapStatusCode(result);
                    break;

                default:
                    break;
            }

            SetResponseHeaders(responseMessage, response);

            if (bytes != null)
            {
                await response.Body.WriteAsync(bytes, 0, bytes.Length);
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

        private bool IsFault(ResponseMessage responseMessage)
        {
            return responseMessage.FaultPercentage == null || _randomizerDouble.Generate() <= responseMessage.FaultPercentage;
        }

        private byte[] GetNormalBody(ResponseMessage responseMessage)
        {
            byte[] bytes = null;
            switch (responseMessage.BodyData?.DetectedBodyType)
            {
                case BodyType.String:
                    bytes = (responseMessage.BodyData.Encoding ?? _utf8NoBom).GetBytes(responseMessage.BodyData.BodyAsString);
                    break;

                case BodyType.Json:
                    Formatting formatting = responseMessage.BodyData.BodyAsJsonIndented == true
                        ? Formatting.Indented
                        : Formatting.None;
                    string jsonBody = JsonConvert.SerializeObject(responseMessage.BodyData.BodyAsJson, new JsonSerializerSettings { Formatting = formatting, NullValueHandling = NullValueHandling.Ignore });
                    bytes = (responseMessage.BodyData.Encoding ?? _utf8NoBom).GetBytes(jsonBody);
                    break;

                case BodyType.Bytes:
                    bytes = responseMessage.BodyData.BodyAsBytes;
                    break;

                case BodyType.File:
                    bytes = _options.FileSystemHandler.ReadResponseBodyAsFile(responseMessage.BodyData.BodyAsFile);
                    break;
            }

            return bytes;
        }

        private static void SetResponseHeaders(ResponseMessage responseMessage, IResponse response)
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
            foreach (var item in responseMessage.Headers)
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