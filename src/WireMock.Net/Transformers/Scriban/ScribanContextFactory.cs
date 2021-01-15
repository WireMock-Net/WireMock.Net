using WireMock.Handlers;
using WireMock.Types;
using WireMock.Validation;

namespace WireMock.Transformers.Scriban
{
    internal class ScribanContextFactory : ITransformerContextFactory
    {
        private readonly IFileSystemHandler _fileSystemHandler;
        private readonly TransformerType _transformerType;

        public ScribanContextFactory(IFileSystemHandler fileSystemHandler, TransformerType transformerType)
        {
            Check.NotNull(fileSystemHandler, nameof(fileSystemHandler));
            Check.Condition(transformerType, t => t == TransformerType.Scriban || t == TransformerType.ScribanDotLiquid, nameof(transformerType));

            _fileSystemHandler = fileSystemHandler;
            _transformerType = transformerType;
        }

        public ITransformerContext Create()
        {
            return new ScribanContext(_fileSystemHandler, _transformerType);
        }
    }
}