using HandlebarsDotNet;

namespace WireMock.Transformers
{
    interface IHandlebarsContext : ITransformerContext
    {
        IHandlebars Handlebars { get; set; }
    }
}