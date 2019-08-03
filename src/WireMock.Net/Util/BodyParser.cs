using JetBrains.Annotations;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.Matchers;
using WireMock.Types;
using WireMock.Validation;

namespace WireMock.Util
{
    internal static class BodyParser
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private static readonly Encoding[] SupportedBodyAsStringEncodingForMultipart = { Encoding.UTF8, Encoding.ASCII };

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
        private static readonly IDictionary<string, bool> BodyAllowedForMethods = new Dictionary<string, bool>
        {
            { "HEAD", false },
            { "GET", false },
            { "PUT", true },
            { "POST", true },
            { "DELETE", false },
            { "TRACE", false },
            { "OPTIONS", true },
            { "CONNECT", false },
            { "PATCH", true }
        };

        private static readonly IStringMatcher[] MultipartContentTypesMatchers = {
            new WildcardMatcher("multipart/*", true)
        };

        private static readonly IStringMatcher[] JsonContentTypesMatchers = {
            new WildcardMatcher("application/json", true),
            new WildcardMatcher("application/vnd.*+json", true)
        };

        private static readonly IStringMatcher[] TextContentTypeMatchers =
        {
            new WildcardMatcher("text/*", true),
            new RegexMatcher("^application\\/(java|type)script$", true),
            new WildcardMatcher("application/*xml", true),
            new WildcardMatcher("application/x-www-form-urlencoded", true)
        };

        public static bool ParseBodyAsIsValid([CanBeNull] string parseBodyAs)
        {
            return Enum.TryParse(parseBodyAs, out BodyType _);
        }

        public static bool ShouldParseBody([CanBeNull] string method)
        {
            if (String.IsNullOrEmpty(method))
            {
                return false;
            }
            if (BodyAllowedForMethods.TryGetValue(method.ToUpper(), out var allowed))
            {
                return allowed;
            }
            // If we don't have any knowledge of this method, we should assume that a body *may*
            // be present, so we should parse it if it is. Therefore, if a new method is added to
            // the HTTP Method Registry, we only really need to add it to BodyAllowedForMethods if
            // we want to make it clear that a body is *not* allowed.
            return true;
        }

        public static BodyType DetectBodyTypeFromContentType([CanBeNull] string contentTypeValue)
        {
            if (string.IsNullOrEmpty(contentTypeValue) || !ContentType.TryParse(contentTypeValue, out ContentType contentType))
            {
                return BodyType.Bytes;
            }

            if (TextContentTypeMatchers.Any(matcher => MatchScores.IsPerfect(matcher.IsMatch(contentType.MimeType))))
            {
                return BodyType.String;
            }

            if (JsonContentTypesMatchers.Any(matcher => MatchScores.IsPerfect(matcher.IsMatch(contentType.MimeType))))
            {
                return BodyType.Json;
            }

            if (MultipartContentTypesMatchers.Any(matcher => MatchScores.IsPerfect(matcher.IsMatch(contentType.MimeType))))
            {
                return BodyType.MultiPart;
            }

            return BodyType.Bytes;
        }

        public static async Task<BodyData> Parse([NotNull] Stream stream, [CanBeNull] string contentType)
        {
            Check.NotNull(stream, nameof(stream));

            var data = new BodyData
            {
                BodyAsBytes = await ReadBytesAsync(stream),
                DetectedBodyType = BodyType.Bytes,
                DetectedBodyTypeFromContentType = DetectBodyTypeFromContentType(contentType)
            };

            // In case of MultiPart: check if the BodyAsBytes is a valid UTF8 or ASCII string, in that case read as String else keep as-is
            if (data.DetectedBodyTypeFromContentType == BodyType.MultiPart)
            {
                if (BytesEncodingUtils.TryGetEncoding(data.BodyAsBytes, out Encoding encoding) &&
                    SupportedBodyAsStringEncodingForMultipart.Select(x => x.Equals(encoding)).Any())
                {
                    data.BodyAsString = encoding.GetString(data.BodyAsBytes);
                    data.Encoding = encoding;
                    data.DetectedBodyType = BodyType.String;

                    return data;
                }

                return data;
            }

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