using FluentAssertions;
using WireMock.Matchers;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class StringUtilsTests
{
    [Theory]
    [InlineData("And", MatchOperator.And)]
    [InlineData("Or", MatchOperator.Or)]
    public void ParseMatchOperator_ShouldReturnCorrectEnumValue_WhenValidStringIsProvided(string value, MatchOperator expected)
    {
        // Arrange & Act
        var result = StringUtils.ParseMatchOperator(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, MatchOperator.Or)]
    [InlineData("", MatchOperator.Or)]
    [InlineData("and", MatchOperator.Or)]
    [InlineData("InvalidValue", MatchOperator.Or)]
    public void ParseMatchOperator_ShouldReturnDefaultEnumValue_WhenInvalidOrNullStringIsProvided(string? value, MatchOperator expected)
    {
        // Arrange & Act
        var result = StringUtils.ParseMatchOperator(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("'s")]
    [InlineData("\"s")]
    public void StringUtils_TryParseQuotedString_With_UnexpectedUnclosedString_Returns_False(string input)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out _, out _);

        // Assert
        valid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("x")]
    public void StringUtils_TryParseQuotedString_With_InvalidStringLength_Returns_False(string input)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out _, out _);

        // Assert
        valid.Should().BeFalse();
    }

    [Theory]
    [InlineData("xx")]
    [InlineData("  ")]
    public void StringUtils_TryParseQuotedString_With_InvalidStringQuoteCharacter_Returns_False(string input)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out _, out _);

        // Assert
        valid.Should().BeFalse();
    }

    [Fact]
    public void StringUtils_TryParseQuotedString_With_UnexpectedUnrecognizedEscapeSequence_Returns_False()
    {
        // Arrange
        var input = new string(new[] { '"', '\\', 'u', '?', '"' });

        // Act
        var valid = StringUtils.TryParseQuotedString(input, out _, out _);

        // Assert
        valid.Should().BeFalse();
    }

    [Theory]
    [InlineData("''", "")]
    [InlineData("'s'", "s")]
    [InlineData("'\\\\'", "\\")]
    [InlineData("'\\n'", "\n")]
    public void StringUtils_TryParseQuotedString_SingleQuotedString(string input, string expectedResult)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out var result, out var quote);

        // Assert
        valid.Should().BeTrue();
        result.Should().Be(expectedResult);
        quote.Should().Be('\'');
    }

    [Theory]
    [InlineData("\"\"", "")]
    [InlineData("\"\\\\\"", "\\")]
    [InlineData("\"\\n\"", "\n")]
    [InlineData("\"\\\\n\"", "\\n")]
    [InlineData("\"\\\\new\"", "\\new")]
    [InlineData("\"[]\"", "[]")]
    [InlineData("\"()\"", "()")]
    [InlineData("\"(\\\"\\\")\"", "(\"\")")]
    [InlineData("\"/\"", "/")]
    [InlineData("\"a\"", "a")]
    [InlineData("\"This \\\"is\\\" a test.\"", "This \"is\" a test.")]
    [InlineData(@"""This \""is\"" b test.""", @"This ""is"" b test.")]
    [InlineData("\"ab\\\"cd\"", "ab\"cd")]
    [InlineData("\"\\\"\"", "\"")]
    [InlineData("\"\\\"\\\"\"", "\"\"")]
    [InlineData("\"AB YZ 19 \uD800\udc05 \u00e4\"", "AB YZ 19 \uD800\udc05 \u00e4")]
    [InlineData("\"\\\\\\\\192.168.1.1\\\\audio\\\\new\"", "\\\\192.168.1.1\\audio\\new")]
    public void StringUtils_TryParseQuotedString_DoubleQuotedString(string input, string expectedResult)
    {
        // Act
        var valid = StringUtils.TryParseQuotedString(input, out var result, out var quote);

        // Assert
        valid.Should().BeTrue();
        result.Should().Be(expectedResult);
        quote.Should().Be('"');
    }
}