using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Serialization;

namespace WireMock.Util;

internal static class JsonUtils
{
    public static bool TryParseAsJObject(string? strInput, [NotNullWhen(true)] out JObject? value)
    {
        value = null;

        if (strInput == null || string.IsNullOrWhiteSpace(strInput))
        {
            return false;
        }

        strInput = strInput.Trim();
        if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) && (!strInput.StartsWith("[") || !strInput.EndsWith("]")))
        {
            return false;
        }

        try
        {
            // Try to convert this string into a JToken
            value = JObject.Parse(strInput);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string Serialize(object value)
    {
        return JsonConvert.SerializeObject(value, JsonSerializationConstants.JsonSerializerSettingsIncludeNullValues);
    }

    public static byte[] SerializeAsPactFile(object value)
    {
        var json = JsonConvert.SerializeObject(value, JsonSerializationConstants.JsonSerializerSettingsPact);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Load a Newtonsoft.Json.Linq.JObject from a string that contains JSON.
    /// Using : DateParseHandling = DateParseHandling.None
    /// </summary>
    /// <param name="json">A System.String that contains JSON.</param>
    /// <returns>A Newtonsoft.Json.Linq.JToken populated from the string that contains JSON.</returns>
    public static JToken Parse(string json)
    {
        return JsonConvert.DeserializeObject<JToken>(json, JsonSerializationConstants.JsonDeserializerSettingsWithDateParsingNone)!;
    }

    /// <summary>
    /// Deserializes the JSON to a .NET object.
    /// Using : DateParseHandling = DateParseHandling.None
    /// </summary>
    /// <param name="json">A System.String that contains JSON.</param>
    /// <returns>The deserialized object from the JSON string.</returns>
    public static object DeserializeObject(string json)
    {
        return JsonConvert.DeserializeObject(json, JsonSerializationConstants.JsonDeserializerSettingsWithDateParsingNone)!;
    }

    /// <summary>
    /// Deserializes the JSON to the specified .NET type.
    /// Using : DateParseHandling = DateParseHandling.None
    /// </summary>
    /// <param name="json">A System.String that contains JSON.</param>
    /// <returns>The deserialized object from the JSON string.</returns>
    public static T DeserializeObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, JsonSerializationConstants.JsonDeserializerSettingsWithDateParsingNone)!;
    }

    public static T? TryDeserializeObject<T>(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            return default;
        }
    }

    public static T ParseJTokenToObject<T>(object? value)
    {
        if (value != null && value.GetType() == typeof(T))
        {
            return (T)value;
        }

        return value switch
        {
            JToken tokenValue => tokenValue.ToObject<T>()!,

            _ => throw new NotSupportedException($"Unable to convert value to {typeof(T)}.")
        };
    }

    /// <summary>
    /// Based on / copied from Handlebars.Net Handlebars.Net.Helpers.Utils.JsonUtils
    /// </summary>
    public static string GenerateDynamicLinqStatement(JToken jsonObject)
    {
        var lines = new List<string>();
        WalkNode(jsonObject, null, null, lines);

        return lines.First();
    }

    private static void WalkNode(JToken node, string? path, string? propertyName, List<string> lines)
    {
        switch (node.Type)
        {
            case JTokenType.Object:
                ProcessObject(node, propertyName, lines);
                break;

            case JTokenType.Array:
                ProcessArray(node, propertyName, lines);
                break;

            default:
                ProcessItem(node, path ?? "it", propertyName, lines);
                break;
        }
    }

    private static void ProcessObject(JToken node, string? propertyName, List<string> lines)
    {
        var items = new List<string>();
        var text = new StringBuilder("new (");

        // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
        foreach (var child in node.Children<JProperty>().ToArray())
        {
            WalkNode(child.Value, child.Path, child.Name, items);
        }

        text.Append(string.Join(", ", items));
        text.Append(")");

        if (!string.IsNullOrEmpty(propertyName))
        {
            text.AppendFormat(" as {0}", propertyName);
        }

        lines.Add(text.ToString());
    }

    private static void ProcessArray(JToken node, string? propertyName, List<string> lines)
    {
        var items = new List<string>();
        var text = new StringBuilder("(new [] { ");

        // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
        var idx = 0;
        foreach (var child in node.Children().ToArray())
        {
            WalkNode(child, $"{node.Path}[{idx}]", null, items);
            idx++;
        }

        text.Append(string.Join(", ", items));
        text.Append("})");

        if (!string.IsNullOrEmpty(propertyName))
        {
            text.AppendFormat(" as {0}", propertyName);
        }

        lines.Add(text.ToString());
    }

    private static void ProcessItem(JToken node, string path, string? propertyName, List<string> lines)
    {
        var castText = node.Type switch
        {
            JTokenType.Boolean => $"bool({path})",
            JTokenType.Date => $"DateTime({path})",
            JTokenType.Float => $"double({path})",
            JTokenType.Guid => $"Guid({path})",
            JTokenType.Integer => $"long({path})",
            JTokenType.Null => "null",
            JTokenType.String => $"string({path})",
            JTokenType.TimeSpan => $"TimeSpan({path})",
            JTokenType.Uri => $"Uri({path})",
            _ => throw new NotSupportedException($"JTokenType '{node.Type}' cannot be converted to a Dynamic Linq cast operator.")
        };

        if (!string.IsNullOrEmpty(propertyName))
        {
            castText += $" as {propertyName}";
        }

        lines.Add(castText);
    }
}