using HandlebarsDotNet;
using System;
using WireMock.Handlers;

namespace WireMock.Transformers
{
    internal class HandlebarsContextFactory : IHandlebarsContextFactory
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

        public IHandlebarsContext Create()
        {
            var handlebars = Handlebars.Create(HandlebarsConfiguration);

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