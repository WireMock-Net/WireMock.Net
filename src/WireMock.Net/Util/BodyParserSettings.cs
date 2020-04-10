using System.IO;

namespace WireMock.Util
{
    internal class BodyParserSettings
    {
        public Stream Stream { get; set; }

        public string ContentType { get; set; }

        public string ContentEncoding { get; set; }

        public bool DecompressGZipAndDeflate { get; set; } = true;

        public bool DeserializeJson { get; set; } = true;
    }
}