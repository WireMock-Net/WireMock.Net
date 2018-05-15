using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace WireMock.Util
{
    internal static class BodyParser
    {
        private static readonly string[] TextContentTypes =
        {
            "text/",
            "application/javascript", "application/typescript",
            "application/xml", "application/xhtml+xml",
            "application/x-www-form-urlencoded"
        };

        private static async Task<Tuple<string, Encoding>> ReadStringAsync(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                string content = await streamReader.ReadToEndAsync();

                return new Tuple<string, Encoding>(content, streamReader.CurrentEncoding);
            }
        }

        private static async Task<byte[]> ReadBytesAsync(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static async Task<BodyData> Parse([NotNull] Stream stream, [CanBeNull] string contentTypeHeaderValue)
        {
            var data = new BodyData();

            if (contentTypeHeaderValue != null && TextContentTypes.Any(t => contentTypeHeaderValue.StartsWith(t, StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var stringData = await ReadStringAsync(stream);
                    data.BodyAsString = stringData.Item1;
                    data.Encoding = stringData.Item2;
                }
                catch
                {
                    // Reading as string failed, just get the ByteArray.
                    data.BodyAsBytes = await ReadBytesAsync(stream);
                }
            }
            else if (contentTypeHeaderValue != null && contentTypeHeaderValue.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                var stringData = await ReadStringAsync(stream);
                data.BodyAsString = stringData.Item1;
                data.Encoding = stringData.Item2;

                try
                {
                    data.BodyAsJson = JsonConvert.DeserializeObject(stringData.Item1, new JsonSerializerSettings { Formatting = Formatting.Indented });
                }
                catch
                {
                    // JsonConvert failed, just set the Body as string.
                    data.BodyAsString = stringData.Item1;
                }
            }
            else
            {
                data.BodyAsBytes = await ReadBytesAsync(stream);
            }

            return data;
        }
    }
}