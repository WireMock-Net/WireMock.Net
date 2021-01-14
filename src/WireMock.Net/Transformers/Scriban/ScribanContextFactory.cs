using WireMock.Handlers;
using WireMock.Types;

namespace WireMock.Transformers.Scriban
{
    internal class ScribanContextFactory : ITransformerContextFactory<ScribanContext>
    {
        private readonly IFileSystemHandler _fileSystemHandler;
        private readonly TransformerType _transformerType;

        public ScribanContextFactory(IFileSystemHandler fileSystemHandler, TransformerType transformerType)
        {
            _fileSystemHandler = fileSystemHandler;
            _transformerType = transformerType;
        }

        public ScribanContext Create()
        {
            return new ScribanContext(_fileSystemHandler, _transformerType);
        }
    }
}