using System;
using Newtonsoft.Json.Linq;
using Scriban;
using Scriban.Runtime;
using WireMock.Handlers;
using WireMock.Types;

namespace WireMock.Transformers.Scriban
{
    internal class ScribanContext : ITransformerContext
    {
        //private readonly ParserOptions options = new ParserOptions
        //{

        //};

        //private readonly LexerOptions options = new LexerOptions
        //{

        //};

        private readonly TransformerType _transformerType;

        public IFileSystemHandler FileSystemHandler { get; set; }

        public ScribanContext(IFileSystemHandler fileSystemHandler, TransformerType transformerType)
        {
            FileSystemHandler = fileSystemHandler ?? throw new ArgumentNullException(nameof(fileSystemHandler));
            _transformerType = transformerType;
        }

        public string ParseAndRender(string text, object model)
        {
            var template = _transformerType == TransformerType.ScribanDotLiquid ? Template.ParseLiquid(text) : Template.Parse(text);

            //switch (model)
            //{
            //    case JObject jobject:
            //        model = ConvertFromJson(jobject);
            //        break;

            //    default:
            //        break;
            //}

            var scriptObject = new ScriptObject();
            scriptObject.Import(model);

            // Setup a default renamer at the `TemplateContext` level
            var context = new TemplateContext { MemberRenamer = member => member.Name };
            context.PushGlobal(scriptObject);

            return template.Render(context);

            //return template.Render(model, member => member.Name);
        }

        // https://github.com/scriban/scriban/issues/246
        private static object ConvertFromJson(JObject element)
        {
            switch (element.Type)
            {
                case JTokenType.Object:
                    var obj = new ScriptObject();
                    foreach (var prop in element.Properties())
                    {
                        //Console.WriteLine(prop.Name + ": " + prop.Value.ToString());
                        obj[prop.Name] = prop.Value.ToObject<object>();
                    }

                    return obj;
                case JTokenType.Array:
                    var array = new ScriptArray();
                    foreach (var nestedElement in element.Descendants())
                    {
                        array.Add(ConvertFromJson(JObject.Parse(nestedElement.ToString())));
                    }
                    return array;
                case JTokenType.String:
                    return element.ToString();
                case JTokenType.Integer:
                    return element.ToObject<int>();
                case JTokenType.Float:
                    return element.ToObject<double>();
                case JTokenType.Boolean:
                    return element.ToObject<bool>();
                default:
                    return null;
            }
        }
    }
}