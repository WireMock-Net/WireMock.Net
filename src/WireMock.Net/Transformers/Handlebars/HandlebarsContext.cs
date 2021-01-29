using HandlebarsDotNet;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars
{
    internal class HandlebarsContext : IHandlebarsContext
    {
        public IHandlebars Handlebars { get; set; }

        public IFileSystemHandler FileSystemHandler { get; set; }

        public string ParseAndRender(string text, object model)
        {
            var template = Handlebars.Compile(text);
            return template(model);
            //var reader = new StringReader(text);
            //var template = Handlebars.Compile(reader);

            //var writer = new StringWriter();

            //template(writer, model);

            //switch (result)
            //{
            //    case JToken jTokenResult:
            //        return jTokenResult.ToString();

            //    case string stringResult:
            //        return stringResult;

            //    default:
            //        return result.ToString();
            //}
        }
    }
}