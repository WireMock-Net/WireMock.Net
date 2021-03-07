namespace WireMock.Models
{
    public interface IWebhook
    { 
        IWebhookRequest Request { get; set; }
    }
}