namespace WireMock.Org.Abstractions
{
    public class NearMiss
    {
        /// <summary>
        /// The HTTP request method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The path and query to match exactly against
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The full URL to match against
        /// </summary>
        public string AbsoluteUrl { get; set; }

        /// <summary>
        /// Header patterns to match against in the &lt;key&gt;: { "&lt;predicate&gt;": "&lt;value&gt;" } form
        /// </summary>
        public object Headers { get; set; }

        /// <summary>
        /// Cookie patterns to match against in the &lt;key&gt;: { "&lt;predicate&gt;": "&lt;value&gt;" } form
        /// </summary>
        public object Cookies { get; set; }

        /// <summary>
        /// Body string to match against
        /// </summary>
        public string Body { get; set; }
    }
}
