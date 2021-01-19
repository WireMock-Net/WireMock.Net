using HandlebarsDotNet;
using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using WireMock.Validation;
#if !NETSTANDARD1_3
using Wmhelp.XPath2;
#endif

namespace WireMock.Transformers.Handlebars
{
    internal static class HandlebarsXPath
    {
        public static void Register(IHandlebars handlebarsContext)
        {
            handlebarsContext.RegisterHelper("XPath.SelectSingleNode", (writer, context, arguments) =>
            {
                (XPathNavigator nav, string xpath) = ParseArguments(arguments);

                try
                {
#if NETSTANDARD1_3
                    var result = nav.SelectSingleNode(xpath);
#else
                    var result = nav.XPath2SelectSingleNode(xpath);
#endif
                    writer.WriteSafeString(result.OuterXml);
                }
                catch (Exception)
                {
                    // Ignore Exception
                }
            });

            handlebarsContext.RegisterHelper("XPath.SelectNodes", (writer, context, arguments) =>
            {
                (XPathNavigator nav, string xpath) = ParseArguments(arguments);

                try
                {
#if NETSTANDARD1_3
                    var result = nav.Select(xpath);
#else
                    var result = nav.XPath2SelectNodes(xpath);
#endif
                    var resultXml = new StringBuilder();
                    foreach (XPathNavigator node in result)
                    {
                        resultXml.Append(node.OuterXml);
                    }

                    writer.WriteSafeString(resultXml);
                }
                catch (Exception)
                {
                    // Ignore Exception
                }
            });

            handlebarsContext.RegisterHelper("XPath.Evaluate", (writer, context, arguments) =>
            {
                (XPathNavigator nav, string xpath) = ParseArguments(arguments);

                try
                {
#if NETSTANDARD1_3
                    var result = nav.Evaluate(xpath);
#else
                    var result = nav.XPath2Evaluate(xpath);
#endif
                    writer.WriteSafeString(result);
                }
                catch (Exception)
                {
                    // Ignore Exception
                }
            });
        }

        private static (XPathNavigator nav, string xpath) ParseArguments(object[] arguments)
        {
            Check.Condition(arguments, args => args.Length == 2, nameof(arguments));
            Check.NotNull(arguments[0], "arguments[0]");
            Check.NotNullOrEmpty(arguments[1] as string, "arguments[1]");

            XPathNavigator nav;

            switch (arguments[0])
            {
                case string stringValue:
                    nav = new XmlDocument { InnerXml = stringValue }.CreateNavigator();
                    break;

                default:
                    throw new NotSupportedException($"The value '{arguments[0]}' with type '{arguments[0]?.GetType()}' cannot be used in Handlebars XPath.");
            }

            return (nav, (string)arguments[1]);
        }
    }
}