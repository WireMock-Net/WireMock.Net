using NFluent;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.Settings
{
    public class SimpleCommandLineParserTests
    {
        private readonly SimpleCommandLineParser _parser;

        public SimpleCommandLineParserTests()
        {
            _parser = new SimpleCommandLineParser();
        }

        [Fact]
        public void SimpleCommandLineParser_Parse_Arguments()
        {
            // Assign
            _parser.Parse(new[] { "--test1", "one", "--test2", "two", "--test3", "three" });

            // Act
            string value1 = _parser.GetStringValue("test1");
            string value2 = _parser.GetStringValue("test2");
            string value3 = _parser.GetStringValue("test3");

            // Assert
            Check.That(value1).IsEqualTo("one");
            Check.That(value2).IsEqualTo("two");
            Check.That(value3).IsEqualTo("three");
        }

        [Fact]
        public void SimpleCommandLineParser_Parse_ArgumentsAsCombinedKeyAndValue()
        {
            // Assign
            _parser.Parse(new[] { "--test1 one", "--test2 two", "--test3 three" });

            // Act
            string value1 = _parser.GetStringValue("test1");
            string value2 = _parser.GetStringValue("test2");
            string value3 = _parser.GetStringValue("test3");

            // Assert
            Check.That(value1).IsEqualTo("one");
            Check.That(value2).IsEqualTo("two");
            Check.That(value3).IsEqualTo("three");
        }

        [Fact]
        public void SimpleCommandLineParser_Parse_ArgumentsMixed()
        {
            // Assign
            _parser.Parse(new[] { "--test1 one", "--test2", "two", "--test3 three" });

            // Act
            string value1 = _parser.GetStringValue("test1");
            string value2 = _parser.GetStringValue("test2");
            string value3 = _parser.GetStringValue("test3");

            // Assert
            Check.That(value1).IsEqualTo("one");
            Check.That(value2).IsEqualTo("two");
            Check.That(value3).IsEqualTo("three");
        }

        [Fact]
        public void SimpleCommandLineParser_Parse_GetBoolValue()
        {
            // Assign
            _parser.Parse(new[] { "'--test1", "false'", "--test2 true" });

            // Act
            bool value1 = _parser.GetBoolValue("test1");
            bool value2 = _parser.GetBoolValue("test2");
            bool value3 = _parser.GetBoolValue("test3", true);

            // Assert
            Check.That(value1).IsEqualTo(false);
            Check.That(value2).IsEqualTo(true);
            Check.That(value3).IsEqualTo(true);
        }

        [Fact]
        public void SimpleCommandLineParser_Parse_GetIntValue()
        {
            // Assign
            _parser.Parse(new[] { "--test1", "42", "--test2 55" });

            // Act
            int? value1 = _parser.GetIntValue("test1");
            int? value2 = _parser.GetIntValue("test2");
            int? value3 = _parser.GetIntValue("test3", 100);
            int? value4 = _parser.GetIntValue("test4");

            // Assert
            Check.That(value1).IsEqualTo(42);
            Check.That(value2).IsEqualTo(55);
            Check.That(value3).IsEqualTo(100);
            Check.That(value4).IsNull();
        }
    }
}