using System;
using Scriban;
using WireMock.Handlers;
using WireMock.Types;

namespace WireMock.Transformers
{
    internal class ScribanContext : ITransformerContext
    {
        private readonly TransformerType _transformerType;

        public IFileSystemHandler FileSystemHandler { get; set; }

        public ScribanContext(IFileSystemHandler fileSystemHandler, TransformerType transformerType)
        {
            FileSystemHandler = fileSystemHandler ?? throw new ArgumentNullException(nameof(fileSystemHandler));
            _transformerType = transformerType;
        }

        public Template Parse(string text)
        {
            return _transformerType == TransformerType.ScribanDotLiquid ? Template.ParseLiquid(text) : Template.Parse(text);
        }
    }
}