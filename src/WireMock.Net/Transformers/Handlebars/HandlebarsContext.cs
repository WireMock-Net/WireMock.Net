using HandlebarsDotNet;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars
{
    internal class HandlebarsContext : IHandlebarsContext
    {
        public IHandlebars Handlebars { get; set; }

        public IFileSystemHandler FileSystemHandler { get; set; }

        public string ParseAndRender(string text, object model)
        {
            var template = Handlebars.Compile(text);
            return template(model);
        }
    }
}