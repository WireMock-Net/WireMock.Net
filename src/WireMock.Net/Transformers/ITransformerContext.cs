// Copyright © WireMock.Net

using WireMock.Handlers;

namespace WireMock.Transformers;

internal interface ITransformerContext
{
    IFileSystemHandler FileSystemHandler { get; }

    string ParseAndRender(string text, object model);

    object? ParseAndEvaluate(string text, object model);
}