namespace WireMock.Org.Abstractions
{
    /// <summary>
    /// Pre-emptive basic auth credentials to match against
    /// </summary>
    public class BasicAuthCredentials
    {
        public string Password { get; set; }

        public string Username { get; set; }
    }
}
