using System.Text;
using WireMock.Types;

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

        /// <summary>
        /// Gets or sets a value indicating whether child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation and Newtonsoft.Json.JsonTextWriter.IndentChar settings.
        /// </summary>
        public bool? BodyAsJsonIndented { get; set; }

        /// <summary>
        /// Gets or sets the body as a file.
        /// </summary>
        public string BodyAsFile { get; set; }

        /// <summary>
        /// Is the body as file cached?
        /// </summary>
        public bool? BodyAsFileIsCached { get; set; }

        /// <summary>
        /// The detected body type (detection based on body content).
        /// </summary>
        public BodyType DetectedBodyType { get; set; }

        /// <summary>
        /// The detected body type (detection based on Content-Type).
        /// </summary>
        public BodyType DetectedBodyTypeFromContentType { get; set; }

        /// <summary>
        /// The detected compression.
        /// </summary>
        public string DetectedCompression { get; set; }
    }
}