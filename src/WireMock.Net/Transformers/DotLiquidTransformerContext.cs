using System;
using WireMock.Handlers;

namespace WireMock.Transformers
{
    internal class DotLiquidTransformerContext : ITransformerContext
    {
        public IFileSystemHandler FileSystemHandler { get; set; }

        public DotLiquidTransformerContext(IFileSystemHandler fileSystemHandler)
        {
            FileSystemHandler = fileSystemHandler ?? throw new ArgumentNullException(nameof(fileSystemHandler));
        }
    }
}