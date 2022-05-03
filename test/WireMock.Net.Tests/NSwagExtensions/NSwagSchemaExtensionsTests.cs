using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.NSwagExtensions;
using Xunit;
using static Humanizer.In;

namespace WireMock.Net.Tests.NSwagExtensions;

public class NSwagSchemaExtensionsTests
{
    [Fact]
    public void JObjectToJsonSchema()
    {
        // Arrange
        var instance = new JObject
        {
            {"Uri", new JValue(new Uri("http://localhost:80/abc?a=5"))},
            {"Null", new JValue((object?) null)},
            {"Guid", new JValue(Guid.NewGuid())},
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
            {"Char", new JValue('c')}
        };

        // Act
        var schema = instance.ToJsonSchema().ToJson(Formatting.Indented).Replace("  ", "    ");

        // Assert
        schema.Should().Be(File.ReadAllText(Path.Combine("../../../", "NSwagExtensions", "JObject.json")));
    }
}