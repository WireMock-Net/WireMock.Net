using System.IO;
#if !USE_ASPNETCORE
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Util
{
    internal class BodyParserSettings
    {
        public Stream Stream { get; set; }

        public string ContentType { get; set; }

        public string ContentEncoding { get; set; }

        public bool DecompressGZipAndDeflate { get; set; } = true;

        public bool DeserializeJson { get; set; } = true;

        public IFormCollection Form { get; set; }
    }
}