using WireMock.Handlers;
using WireMock.Types;
using Stef.Validation;

namespace WireMock.Transformers.Scriban
{
    internal class ScribanContextFactory : ITransformerContextFactory
    {
        private readonly IFileSystemHandler _fileSystemHandler;
        private readonly TransformerType _transformerType;

        public ScribanContextFactory(IFileSystemHandler fileSystemHandler, TransformerType transformerType)
        {
            _fileSystemHandler = Guard.NotNull(fileSystemHandler);
            _transformerType = Guard.Condition(transformerType, t => t == TransformerType.Scriban || t == TransformerType.ScribanDotLiquid);
        }

        public ITransformerContext Create()
        {
            return new ScribanContext(_fileSystemHandler, _transformerType);
        }
    }
}