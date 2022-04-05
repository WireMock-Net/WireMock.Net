namespace WireMock.Pact.Models.V2
{
    public class Interaction
    {
        public string Description { get; set; } = string.Empty;

        public string ProviderState { get; set; }

        public Request Request { get; set; } = new Request();

        public Response Response { get; set; } = new Response();
    }
}