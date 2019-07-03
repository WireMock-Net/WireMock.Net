using HandlebarsDotNet;

namespace WireMock.Transformers
{
    interface IHandlebarsContextFactory
    {
        IHandlebars Create();
    }
}
