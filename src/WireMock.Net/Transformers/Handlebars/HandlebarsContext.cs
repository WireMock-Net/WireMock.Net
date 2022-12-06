using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Handlers;
using WireMock.Types;

namespace WireMock.Transformers.Handlebars;

internal class HandlebarsContext : IHandlebarsContext
{
    public IHandlebars Handlebars { get; }

    public IFileSystemHandler FileSystemHandler { get; }

    public HandlebarsContext(IHandlebars handlebars, IFileSystemHandler fileSystemHandler)
    {
        Handlebars = Guard.NotNull(handlebars);
        FileSystemHandler = Guard.NotNull(fileSystemHandler);
    }

    public string ParseAndRender(string text, object model)
    {
        var template = Handlebars.Compile(text);
        return template(model);
    }

    public object? ParseAndEvaluate(string text, object model)
    {
        if (Handlebars.TryEvaluate(text, model, out var result))
        {
            return result;
        }

        return ParseAndRender(text, model);

        //object? result = null;
        //try
        //{
        //    result = Handlebars.Evaluate(text, model);
        //}
        //catch
        //{
        //    // Ignore exception, and set keep the result on null.
        //}

        // In case Evaluate
        // - throws exception
        // - returns null
        // - returns undefined
        // - returns JObject or alike
        // - returns WireMockList
        //
        // Call ParseAndRender
        return result
            is null
            or UndefinedBindingResult
            or WireMockList<string>
            or JObject
            or JValue
            or JArray
            or JToken
            ? ParseAndRender(text, model)
            : result;
    }
}