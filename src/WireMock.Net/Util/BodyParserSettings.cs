using System.IO;

namespace WireMock.Util
{
    internal class BodyParserSettings
    {
        public Stream Stream { get; set; }

        public string ContentType { get; set; } = null;

        public string ContentEncoding { get; set; } = null;

        public bool DecompressGzipAndDeflate { get; set; } = true;

        public bool DeserializeJson { get; set; } = true;
    }
}