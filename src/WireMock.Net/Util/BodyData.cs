using System.Text;

namespace WireMock.Util
{
    public class BodyData
    {
        public Encoding Encoding { get; set; }

        public string BodyAsString { get; set; }

        public object BodyAsJson { get; set; }

        public byte[] BodyAsBytes { get; set; }
    }
}