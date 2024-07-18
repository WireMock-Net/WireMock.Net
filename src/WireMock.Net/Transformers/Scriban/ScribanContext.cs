// Copyright © WireMock.Net

using Scriban;
using Stef.Validation;
using WireMock.Handlers;
using WireMock.Types;

namespace WireMock.Transformers.Scriban;

internal class ScribanContext : ITransformerContext
{
    private readonly TransformerType _transformerType;

    public IFileSystemHandler FileSystemHandler { get; }

    public ScribanContext(IFileSystemHandler fileSystemHandler, TransformerType transformerType)
    {
        FileSystemHandler = Guard.NotNull(fileSystemHandler);
        _transformerType = transformerType;
    }

    public string ParseAndRender(string text, object model)
    {
        var template = _transformerType == TransformerType.ScribanDotLiquid ? Template.ParseLiquid(text) : Template.Parse(text);

        return template.Render(model, member => member.Name);
    }

    public object? ParseAndEvaluate(string text, object model)
    {
        // In case of Scriban, call ParseAndRender.
        return ParseAndRender(text, model);
    }
}