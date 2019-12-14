using JetBrains.Annotations;

namespace WireMock.Settings
{
    public class WebProxySettings : IWebProxySettings
    {
        /// <inheritdoc cref="IWebProxySettings.Address"/>
        [PublicAPI]
        public string Address { get; set; }

        /// <inheritdoc cref="IWebProxySettings.UserName"/>
        [PublicAPI]
        public string UserName { get; set; }

        /// <inheritdoc cref="IWebProxySettings.Password"/>
        [PublicAPI]
        public string Password { get; set; }
    }
}