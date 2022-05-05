using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.NSwagExtensions;
using Xunit;

namespace WireMock.Net.Tests.NSwagExtensions;

public class NJsonSchemaExtensionsTests
{
    private readonly Guid _guid = new("9579ec16-0f66-486c-a056-2f89f2e0c2dc");
    private readonly byte[] _bytes = { 1, 2, 3 };

    [Fact]
    public void JObjectToJsonSchema()
    {
        // Arrange
        var instance = new JObject
        {
            {"Uri", new JValue(new Uri("http://localhost:80/abc?a=5"))},
            {"Null", new JValue((object?) null)},
            {"Guid", new JValue(_guid)},
            {"Float", new JValue(10.0f)},
            {"Double", new JValue(Math.PI)},
            {"Check", new JValue(true)},
            {
                "Child", new JObject
                {
                    {"ChildInteger", new JValue(4)},
                    {"ChildDateTime", new JValue(new DateTime(2018, 2, 17))},
                    {"ChildTimeSpan", new JValue(TimeSpan.FromMilliseconds(999))}
                }
            },
            {"Integer", new JValue(9)},
            {"Long", new JValue(long.MaxValue)},
            {"String", new JValue("Test")},
            {"Char", new JValue('c')},
            {"Bytes", new JValue(_bytes)},
            {"Array", new JArray("a1")}
        };

        // Act
        var schema = instance.ToJsonSchema().ToJson(Formatting.Indented).Replace("  ", "    ");

        // Assert
        schema.Should().Be(File.ReadAllText(Path.Combine("../../../", "NSwagExtensions", "JObject.json")));
    }

    [Fact]
    public void JArrayToJsonSchema()
    {
        // Arrange
        var instance = new JArray("a1", "a2");

        // Act
        var schema = instance.ToJsonSchema().ToJson(Formatting.Indented).Replace("  ", "    ");

        // Assert
        schema.Should().Be(File.ReadAllText(Path.Combine("../../../", "NSwagExtensions", "JArray.json")));
    }

    [Fact]
    public void ArrayToJsonSchema()
    {
        // Arrange
        var instance = new[] { "a1" };

        // Act
        var schema = instance.ToJsonSchema().ToJson(Formatting.Indented).Replace("  ", "    ");

        // Assert
        schema.Should().Be(File.ReadAllText(Path.Combine("../../../", "NSwagExtensions", "array.json")));
    }

    [Fact]
    public void ObjectToJsonSchema()
    {
        // Arrange
        var instance = new
        {
            Uri = new Uri("http://localhost:80/abc?a=5"),
            Null = (object?)null,
            Guid = _guid,
            Float = 10.0f,
            Double = Math.E,
            Check = true,
            Child = new
            {
                ChildInteger = 4,
                ChildDateTime = new DateTime(2018, 2, 17),
                ChildTimeSpan = TimeSpan.FromMilliseconds(999)
            },
            Integer = 9,
            Long = long.MaxValue,
            String = "test",
            Char = 'c',
            Bytes = _bytes,
            Array = new[] { "a1" },
            ListT = new List<int> { 1 },
            IList = (IList) new List<int> { 1 },
            IEnumerableT = (IEnumerable<string>) new [] { "s" },
            IEnumerable = (IEnumerable) new[] { "s" }
        };

        // Act
        var schema = instance.ToJsonSchema().ToJson(Formatting.Indented).Replace("  ", "    ");

        // Assert
        schema.Should().Be(File.ReadAllText(Path.Combine("../../../", "NSwagExtensions", "object.json")));
    }
}