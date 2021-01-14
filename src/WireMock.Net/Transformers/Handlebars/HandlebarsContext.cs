using HandlebarsDotNet;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars
{
    internal class HandlebarsContext : IHandlebarsContext
    {
        public IHandlebars Handlebars { get; set; }

        public IFileSystemHandler FileSystemHandler { get; set; }
    }
}