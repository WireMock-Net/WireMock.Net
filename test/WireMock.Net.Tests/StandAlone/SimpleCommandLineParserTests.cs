using NFluent;
using WireMock.Net.StandAlone;
using Xunit;

namespace WireMock.Net.Tests
{
    public class SimpleCommandLineParserTests
    {
        SimpleCommandLineParser _parser;

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
        public void SimpleCommandLineParser_Parse_ArgumentsWithSingleQuotes()
        {
            // Assign
            _parser.Parse(new[] { "'--test1", "one'", "'--test2", "two'", "'--test3", "three'" });

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
            _parser.Parse(new[] { "'--test1", "one'", "--test2", "two", "--test3", "three" });

            // Act
            string value1 = _parser.GetStringValue("test1");
            string value2 = _parser.GetStringValue("test2");
            string value3 = _parser.GetStringValue("test3");

            // Assert
            Check.That(value1).IsEqualTo("one");
            Check.That(value2).IsEqualTo("two");
            Check.That(value3).IsEqualTo("three");
        }
    }
}