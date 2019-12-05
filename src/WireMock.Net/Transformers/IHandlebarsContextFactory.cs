using HandlebarsDotNet;

namespace WireMock.Transformers
{
    interface IHandlebarsContextFactory
    {
        IHandlebarsContext Create();
    }
}
