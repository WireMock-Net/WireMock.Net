namespace WireMock.Settings
{
    /// <summary>
    /// IWebProxySettings
    /// </summary>
    public interface IWebProxySettings
    {
        /// <summary>
        /// A string instance that contains the address of the proxy server.
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// The user name associated with the credentials.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// The password for the user name associated with the credentials.
        /// </summary>
        string Password { get; set; }
    }
}
