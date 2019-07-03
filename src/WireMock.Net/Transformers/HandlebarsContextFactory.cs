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

        public IHandlebars Create()
        {
            var handlebarsContext = Handlebars.Create(HandlebarsConfiguration);

            HandlebarsHelpers.Register(handlebarsContext, _fileSystemHandler);

            _action?.Invoke(handlebarsContext, _fileSystemHandler);

            return handlebarsContext;
        }
    }
}
