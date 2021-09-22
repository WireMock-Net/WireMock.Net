using WireMock.Handlers;

namespace WireMock.Transformers
{
    interface ITransformerContext
    {
        IFileSystemHandler FileSystemHandler { get; set; }

        string ParseAndRender(string text, object model);
    }
}