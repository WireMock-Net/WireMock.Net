using System;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Attributes;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers.Helpers;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars
{
    internal class FileHelpers : BaseHelpers, IHelpers
    {
        private readonly IFileSystemHandler _fileSystemHandler;

        public FileHelpers(IHandlebars context, IFileSystemHandler fileSystemHandler) : base(context)
        {
            _fileSystemHandler = fileSystemHandler ?? throw new ArgumentNullException(nameof(fileSystemHandler));
        }

        [HandlebarsWriter(WriterType.String)]
        public string File(object context, string path)
        {
            var templateFunc = Context.Compile(path);
            string transformed = templateFunc(context);
            return _fileSystemHandler.ReadResponseBodyAsString(transformed);
        }
    }

    //internal static class HandlebarsFile
    //{
    //    public static void Register(IHandlebars handlebarsContext, IFileSystemHandler fileSystemHandler)
    //    {
    //        handlebarsContext.RegisterHelper("File", (writer, context, arguments) =>
    //        {
    //            string value = ParseArgumentAndReadFileFragment(handlebarsContext, context, fileSystemHandler, arguments);
    //            writer.Write(value);
    //        });

    //        handlebarsContext.RegisterHelper("File", (writer, options, context, arguments) =>
    //        {
    //            string value = ParseArgumentAndReadFileFragment(handlebarsContext, context, fileSystemHandler, arguments);
    //            options.Template(writer, value);
    //        });
    //    }

    //    private static string ParseArgumentAndReadFileFragment(IHandlebars handlebarsContext, dynamic context, IFileSystemHandler fileSystemHandler, object[] arguments)
    //    {
    //        Check.Condition(arguments, args => args.Length == 1, nameof(arguments));
    //        Check.NotNull(arguments[0], "arguments[0]");

    //        switch (arguments[0])
    //        {
    //            case string path:
    //                var templateFunc = handlebarsContext.Compile(path);
    //                string transformed = templateFunc(context);
    //                return fileSystemHandler.ReadResponseBodyAsString(transformed);
    //        }

    //        throw new NotSupportedException($"The value '{arguments[0]}' with type '{arguments[0]?.GetType()}' cannot be used in Handlebars File.");
    //    }
    //}
}