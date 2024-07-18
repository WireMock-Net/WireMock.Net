// Copyright © WireMock.Net

using System;
using FluentAssertions;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class JsonUtilsTests
{
    [Fact]
    public void JsonUtils_ParseJTokenToObject_For_String()
    {
        // Assign
        object value = "test";

        // Act
        string result = JsonUtils.ParseJTokenToObject<string>(value);

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void JsonUtils_ParseJTokenToObject_For_Int()
    {
        // Assign
        object value = 123;

        // Act
        var result = JsonUtils.ParseJTokenToObject<int>(value);

        // Assert
        result.Should().Be(123);
    }

    [Fact]
    public void JsonUtils_ParseJTokenToObject_For_Invalid_Throws()
    {
        // Assign
        object value = "{ }";

        // Act
        Action action = () => JsonUtils.ParseJTokenToObject<int>(value);

        // Assert
        action.Should().Throw<NotSupportedException>();
    }
}