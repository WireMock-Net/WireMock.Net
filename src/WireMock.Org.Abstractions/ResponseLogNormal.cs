namespace WireMock.Org.Abstractions
{
    /// <summary>
    /// Log normal randomly distributed response delay.
    /// </summary>
    public class ResponseLogNormal
    {
        public int Median { get; set; }

        public double Sigma { get; set; }

        public string Type { get; set; }
    }
}
