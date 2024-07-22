// Copyright © WireMock.Net

using HandlebarsDotNet;

namespace WireMock.Transformers.Handlebars;

internal interface IHandlebarsContext : ITransformerContext
{
    IHandlebars Handlebars { get; }
}