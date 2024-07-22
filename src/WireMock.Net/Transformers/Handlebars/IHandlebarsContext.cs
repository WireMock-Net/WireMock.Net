// Copyright © WireMock.Net

using HandlebarsDotNet;

namespace WireMock.Transformers.Handlebars;

interface IHandlebarsContext : ITransformerContext
{
    IHandlebars Handlebars { get; }
}