// Copyright © WireMock.Net

using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Attributes;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers.Helpers;
using Stef.Validation;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars;

internal class FileHelpers : BaseHelpers, IHelpers
{
    private readonly IFileSystemHandler _fileSystemHandler;

    public FileHelpers(IHandlebars context, IFileSystemHandler fileSystemHandler) : base(context)
    {
        _fileSystemHandler = Guard.NotNull(fileSystemHandler);
    }

    [HandlebarsWriter(WriterType.String, usage: HelperUsage.Both, passContext: true, name: "File")]
    public string Read(Context context, string path)
    {
        var templateFunc = Context.Compile(path);
        string transformed = templateFunc(context.Value);
        return _fileSystemHandler.ReadResponseBodyAsString(transformed);
    }
}