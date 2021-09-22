namespace WireMock.Org.Abstractions
{
    /// <summary>
    /// Uniformly distributed random response delay.
    /// </summary>
    public class ResponseLogUniformlyDistributed
    {
        public int Lower { get; set; }

        public string Type { get; set; }

        public int Upper { get; set; }
    }
}
