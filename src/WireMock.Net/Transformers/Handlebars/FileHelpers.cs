// Copyright © WireMock.Net

using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Attributes;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers.Helpers;
using HandlebarsDotNet.Helpers.Options;
using Stef.Validation;
using WireMock.Handlers;
using WireMock.Settings;

namespace WireMock.Transformers.Handlebars;

internal class FileHelpers : BaseHelpers, IHelpers
{
    internal const string Name = "File";

    private readonly IFileSystemHandler _fileSystemHandler;

    public FileHelpers(IHandlebars context, WireMockServerSettings settings) : base(context, new HandlebarsHelpersOptions())
    {
        _fileSystemHandler = Guard.NotNull(settings.FileSystemHandler);
    }

    [HandlebarsWriter(WriterType.String, usage: HelperUsage.Both, passContext: true, name: Name)]
    public string Read(Context context, string path)
    {
        var templateFunc = Context.Compile(path);
        var transformedPath = templateFunc(context.Value);
        return _fileSystemHandler.ReadResponseBodyAsString(transformedPath);
    }

    public Category Category => Category.Custom;
}