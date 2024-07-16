// Copyright © WireMock.Net

namespace WireMock.Org.Abstractions
{
    public class MappingsRequest
    {
        /// <summary>
        /// The HTTP request method e.g. GET
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The path and query to match exactly against. Only one of url, urlPattern, urlPath or urlPathPattern may be specified.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The path to match exactly against. Only one of url, urlPattern, urlPath or urlPathPattern may be specified.
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// The path regex to match against. Only one of url, urlPattern, urlPath or urlPathPattern may be specified.
        /// </summary>
        public string UrlPathPattern { get; set; }

        /// <summary>
        /// The path and query regex to match against. Only one of url, urlPattern, urlPath or urlPathPattern may be specified.
        /// </summary>
        public string UrlPattern { get; set; }

        /// <summary>
        /// Query parameter patterns to match against in the : { "": "" } form
        /// </summary>
        public object QueryParameters { get; set; }

        /// <summary>
        /// Header patterns to match against in the : { "": "" } form
        /// </summary>
        public object Headers { get; set; }

        /// <summary>
        /// Pre-emptive basic auth credentials to match against
        /// </summary>
        public MappingsRequestBasicAuthCredentials BasicAuthCredentials { get; set; }

        /// <summary>
        /// Cookie patterns to match against in the : { "": "" } form
        /// </summary>
        public object Cookies { get; set; }

        /// <summary>
        /// Request body patterns to match against in the : { "": "" } form
        /// </summary>
        public object[] BodyPatterns { get; set; }
    }
}