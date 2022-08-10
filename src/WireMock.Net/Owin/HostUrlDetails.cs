namespace WireMock.Owin;

internal struct HostUrlDetails
{
    public bool IsHttps { get; set; }

    public string Url { get; set; }
        
    public string Protocol { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }
}