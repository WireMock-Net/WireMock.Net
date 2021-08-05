namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// The Webhook
    /// </summary>
#if RESTCLIENT
    [FluentBuilder.AutoGenerateBuilder]
#endif
    public class WebhookModel
    {
        /// <summary>
        /// The Webhook Request.
        /// </summary>
        public WebhookRequestModel Request { get; set; }
    }
}