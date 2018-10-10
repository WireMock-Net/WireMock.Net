using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Validation;

namespace WireMock.Util
{
    internal static class BodyParser
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /*
            HEAD - No defined body semantics.
            GET - No defined body semantics.
            PUT - Body supported.
            POST - Body supported.
            DELETE - No defined body semantics.
            TRACE - Body not supported.
            OPTIONS - Body supported but no semantics on usage (maybe in the future).
            CONNECT - No defined body semantics
            PATCH - Body supported.
        */
        private static readonly string[] AllowedBodyParseMethods = { "PUT", "POST", "OPTIONS", "PATCH" };

        private static readonly string[] JsonContentTypes = {
            "application/json",
            "application/vnd.api+json"
        };

        private static readonly string[] TextContentTypes =
        {
            "text/",
            "application/javascript", "application/typescript",
            "application/xml", "application/xhtml+xml",
            "application/x-www-form-urlencoded"
        };

        public static bool ParseBodyAsIsValid([CanBeNull] string parseBodyAs)
        {
            return Enum.TryParse(parseBodyAs, out BodyType _);
        }

        public static bool ShouldParseBody([CanBeNull] string method)
        {
            return AllowedBodyParseMethods.Contains(method, StringComparer.OrdinalIgnoreCase);
        }

        public static BodyType DetectBodyTypeFromContentType([CanBeNull] string contentTypeHeaderValue)
        {
            if (string.IsNullOrEmpty(contentTypeHeaderValue))
            {
                return BodyType.Bytes;
            }

            if (TextContentTypes.Any(text => contentTypeHeaderValue.StartsWith(text, StringComparison.OrdinalIgnoreCase)))
            {
                return BodyType.String;
            }

            if (JsonContentTypes.Any(text => contentTypeHeaderValue.StartsWith(text, StringComparison.OrdinalIgnoreCase)))
            {
                return BodyType.Json;
            }

            return BodyType.Bytes;
        }

        public static async Task<BodyData> Parse([NotNull] Stream stream, [CanBeNull] string contentTypeHeaderValue)
        {
            Check.NotNull(stream, nameof(stream));

            var data = new BodyData
            {
                BodyAsBytes = await ReadBytesAsync(stream),
                DetectedBodyType = BodyType.Bytes,
                DetectedBodyTypeFromContentType = DetectBodyTypeFromContentType(contentTypeHeaderValue)
            };

            // Try to get the body as String
            try
            {
                data.BodyAsString = DefaultEncoding.GetString(data.BodyAsBytes);
                data.Encoding = DefaultEncoding;
                data.DetectedBodyType = BodyType.String;

                // If string is not null or empty, try to get as Json
                if (!string.IsNullOrEmpty(data.BodyAsString))
                {
                    try
                    {
                        data.BodyAsJson = JsonConvert.DeserializeObject(data.BodyAsString, new JsonSerializerSettings { Formatting = Formatting.Indented });
                        data.DetectedBodyType = BodyType.Json;
                    }
                    catch
                    {
                        // JsonConvert failed, just ignore.
                    }
                }
            }
            catch
            {
                // Reading as string failed, just ignore
            }

            return data;
        }
        private static async Task<byte[]> ReadBytesAsync(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}