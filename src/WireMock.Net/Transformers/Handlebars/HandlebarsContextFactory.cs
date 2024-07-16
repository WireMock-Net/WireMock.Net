// Copyright © WireMock.Net

using HandlebarsDotNet;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.Transformers.Handlebars;

internal class HandlebarsContextFactory : ITransformerContextFactory
{
    private readonly WireMockServerSettings _settings;

    public HandlebarsContextFactory(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
    }

    public ITransformerContext Create()
    {
        var config = new HandlebarsConfiguration
        {
            FormatProvider = _settings.Culture
        };
        var handlebars = HandlebarsDotNet.Handlebars.Create(config);

        WireMockHandlebarsHelpers.Register(handlebars, _settings.FileSystemHandler);

        _settings.HandlebarsRegistrationCallback?.Invoke(handlebars, _settings.FileSystemHandler);

        return new HandlebarsContext(handlebars, _settings.FileSystemHandler);
    }
}