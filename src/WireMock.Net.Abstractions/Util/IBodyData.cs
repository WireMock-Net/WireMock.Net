using System.Text;
using WireMock.Types;

namespace WireMock.Util
{
    /// <summary>
    /// IBodyData
    /// </summary>
    public interface IBodyData
    {
        /// <summary>
        /// The body (as bytearray).
        /// </summary>
        byte[] BodyAsBytes { get; set; }

        /// <summary>
        /// Gets or sets the body as a file.
        /// </summary>
        string BodyAsFile { get; set; }

        /// <summary>
        /// Is the body as file cached?
        /// </summary>
        bool? BodyAsFileIsCached { get; set; }

        /// <summary>
        /// The body (as JSON object).
        /// </summary>
        object BodyAsJson { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation and Newtonsoft.Json.JsonTextWriter.IndentChar settings.
        /// </summary>
        bool? BodyAsJsonIndented { get; set; }

        /// <summary>
        /// The body as string, this is defined when BodyAsString or BodyAsJson are not null.
        /// </summary>
        string BodyAsString { get; set; }

        /// <summary>
        /// The detected body type (detection based on body content).
        /// </summary>
        BodyType DetectedBodyType { get; set; }

        /// <summary>
        /// The detected body type (detection based on Content-Type).
        /// </summary>
        BodyType DetectedBodyTypeFromContentType { get; set; }

        /// <summary>
        /// The detected compression.
        /// </summary>
        string DetectedCompression { get; set; }

        /// <summary>
        /// The body encoding.
        /// </summary>
        Encoding Encoding { get; set; }
    }
}