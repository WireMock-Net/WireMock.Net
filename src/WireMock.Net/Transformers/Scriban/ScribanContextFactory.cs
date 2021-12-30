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
            Guard.NotNull(fileSystemHandler, nameof(fileSystemHandler));
            Guard.Condition(transformerType, t => t == TransformerType.Scriban || t == TransformerType.ScribanDotLiquid, nameof(transformerType));

            _fileSystemHandler = fileSystemHandler;
            _transformerType = transformerType;
        }

        public ITransformerContext Create()
        {
            return new ScribanContext(_fileSystemHandler, _transformerType);
        }
    }
}