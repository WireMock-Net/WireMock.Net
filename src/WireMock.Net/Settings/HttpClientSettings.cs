namespace WireMock.Settings
{
    /// <summary>
    /// HttpClientSettings
    /// </summary>
    public class HttpClientSettings : IHttpClientSettings
    {
        /// <inheritdoc cref="IHttpClientSettings.ClientX509Certificate2ThumbprintOrSubjectName"/>
        public string ClientX509Certificate2ThumbprintOrSubjectName { get; set; }

        /// <inheritdoc cref="IHttpClientSettings.WebProxySettings"/>
        public IWebProxySettings WebProxySettings { get; set; }

        /// <inheritdoc cref="IHttpClientSettings.AllowAutoRedirect"/>
        public bool? AllowAutoRedirect { get; set; }
    }
}