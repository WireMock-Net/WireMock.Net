using System;
using HandlebarsDotNet;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars
{
    internal class HandlebarsContextFactory : ITransformerContextFactory
    {
        private static readonly HandlebarsConfiguration HandlebarsConfiguration = new HandlebarsConfiguration
        {
            UnresolvedBindingFormatter = "{0}"
        };

        private readonly IFileSystemHandler _fileSystemHandler;
        private readonly Action<IHandlebars, IFileSystemHandler> _action;

        public HandlebarsContextFactory(IFileSystemHandler fileSystemHandler, Action<IHandlebars, IFileSystemHandler> action)
        {
            _fileSystemHandler = fileSystemHandler;
            _action = action;
        }

        public ITransformerContext Create()
        {
            var handlebars = HandlebarsDotNet.Handlebars.Create(HandlebarsConfiguration);

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