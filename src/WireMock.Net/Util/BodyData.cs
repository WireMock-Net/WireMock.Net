using System.Text;

namespace WireMock.Util
{
    /// <summary>
    /// BodyData
    /// </summary>
    public class BodyData
    {
        /// <summary>
        /// The body encoding.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// The body as string, this is defined when BodyAsString or BodyAsJson are not null.
        /// </summary>
        public string BodyAsString { get; set; }

        /// <summary>
        /// The body (as JSON object).
        /// </summary>
        public object BodyAsJson { get; set; }

        /// <summary>
        /// The body (as bytearray).
        /// </summary>
        public byte[] BodyAsBytes { get; set; }
    }
}