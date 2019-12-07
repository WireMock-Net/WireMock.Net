using HandlebarsDotNet;
using WireMock.Handlers;

namespace WireMock.Transformers
{
    interface IHandlebarsContext
    {
        IHandlebars Handlebars { get; set; }

        IFileSystemHandler FileSystemHandler { get; set; }
    }
}