namespace WireMock.Models
{
    public class Webhook : IWebhook
    {
        public IWebhookRequest Request { get; set ; }
    }
}
