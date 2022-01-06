using System;
using HandlebarsDotNet;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars
{
    internal class HandlebarsContextFactory : ITransformerContextFactory
    {
        private readonly IFileSystemHandler _fileSystemHandler;
        private readonly Action<IHandlebars, IFileSystemHandler> _action;

        public HandlebarsContextFactory([NotNull] IFileSystemHandler fileSystemHandler, [CanBeNull] Action<IHandlebars, IFileSystemHandler> action)
        {
            _fileSystemHandler = Guard.NotNull(fileSystemHandler);
            _action = action;
        }

        public ITransformerContext Create()
        {
            var handlebars = HandlebarsDotNet.Handlebars.Create();

            WireMockHandlebarsHelpers.Register(handlebars, _fileSystemHandler);

            _action?.Invoke(handlebars, _fileSystemHandler);

            return new HandlebarsContext
            {
                Handlebars = handlebars,
                FileSystemHandler = _fileSystemHandler
            };
        }
    }
}