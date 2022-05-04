using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AnyOfTypes;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace WireMock.NSwagExtensions;

internal static class NSwagSchemaExtensions
{
    private static readonly JsonSchemaProperty Boolean = new() { Type = JsonObjectType.Boolean };
    private static readonly JsonSchemaProperty Byte = new() { Type = JsonObjectType.String, Format = JsonFormatStrings.Byte };
    private static readonly JsonSchemaProperty Date = new() { Type = JsonObjectType.String, Format = JsonFormatStrings.DateTime };
    private static readonly JsonSchemaProperty Float = new() { Type = JsonObjectType.Number, Format = JsonFormatStrings.Float };
    private static readonly JsonSchemaProperty Double = new() { Type = JsonObjectType.Number, Format = JsonFormatStrings.Double };
    private static readonly JsonSchemaProperty Guid = new() { Type = JsonObjectType.String, Format = JsonFormatStrings.Guid };
    private static readonly JsonSchemaProperty Integer = new() { Type = JsonObjectType.Integer, Format = JsonFormatStrings.Integer };
    private static readonly JsonSchemaProperty Long = new() { Type = JsonObjectType.Integer, Format = JsonFormatStrings.Long };
    private static readonly JsonSchemaProperty Null = new() { Type = JsonObjectType.Null };
    private static readonly JsonSchemaProperty Object = new() { Type = JsonObjectType.Object };
    private static readonly JsonSchemaProperty String = new() { Type = JsonObjectType.String };
    private static readonly JsonSchemaProperty TimeSpan = new() { Type = JsonObjectType.String, Format = JsonFormatStrings.TimeSpan };
    private static readonly JsonSchemaProperty Uri = new() { Type = JsonObjectType.String, Format = JsonFormatStrings.Uri };

    public static JsonSchema ToJsonSchema(this JObject instance)
    {
        return ConvertJToken(instance);
    }

    public static JsonSchema ToJsonSchema(this JArray instance)
    {
        return ConvertJToken(instance);
    }

    public static JsonSchema ToJsonSchema(this object instance)
    {
        return ConvertValue(instance);
        //var schema = new JsonSchema();
        //foreach (var propertyInfo in instance.GetType().GetProperties())
        //{
        //    var value = propertyInfo.GetValue(instance);
        //    var jsonSchemaProperty = value != null ? ConvertValue(value) : ConvertType(propertyInfo.PropertyType);
        //    schema.Properties.Add(propertyInfo.Name, jsonSchemaProperty);
        //}

        //return schema;
    }

    private static JsonSchemaProperty ConvertJToken(JToken value)
    {
        var type = value.Type;
        switch (type)
        {
            case JTokenType.Array:
                var arrayItem = value.HasValues ? ConvertJToken(value.First!) : Object;
                return new JsonSchemaProperty
                {
                    Type = JsonObjectType.Array,
                    Items = { arrayItem }
                };

            case JTokenType.Boolean:
                return Boolean;

            case JTokenType.Bytes:
                return Byte;

            case JTokenType.Date:
                return Date;

            case JTokenType.Guid:
                return Guid;

            case JTokenType.Float:
                return value is JValue { Value: double } ? Double : Float;

            case JTokenType.Integer:
                var valueAsLong = value.Value<long>();
                return valueAsLong is > int.MaxValue or < int.MinValue ? Long : Integer;

            case JTokenType.Null:
                return Null;

            case JTokenType.Object:
                var jsonSchemaPropertyForObject = new JsonSchemaProperty { Type = JsonObjectType.Object };
                foreach (var jProperty in ((JObject)value).Properties())
                {
                    jsonSchemaPropertyForObject.Properties.Add(jProperty.Name, ConvertJToken(jProperty.Value));
                }

                return jsonSchemaPropertyForObject;

            case JTokenType.String:
                return String;

            case JTokenType.TimeSpan:
                return TimeSpan;

            case JTokenType.Uri:
                return Uri;

            default:
                return Object;
        }
    }

    private static JsonSchemaProperty ConvertValue(object value)
    {
        switch (value)
        {
            case Array array:
                return new JsonSchemaProperty
                {
                    Type = JsonObjectType.Array,
                    Items = { ConvertType(array.GetType().GetElementType()!) }
                };

            case IList list:
                var genericArguments = list.GetType().GetGenericArguments();

                JsonSchemaProperty listType;
                if (genericArguments.Length > 0)
                {
                    listType = ConvertType(genericArguments[0]);
                }
                else
                {
                    listType = list.Count > 0 ? ConvertValue(list[0]!) : Object;
                }

                return new JsonSchemaProperty
                {
                    Type = JsonObjectType.Array,
                    Items = { listType }
                };

            case IEnumerable<byte>:
                return Byte;

            case bool:
                return Boolean;

            case DateTime:
                return Date;

            case double:
                return Double;

            case System.Guid:
                return Guid;

            case float:
                return Float;

            case short:
            case int:
                return Integer;

            case long:
                return Long;

            case string:
                return String;

            case System.TimeSpan:
                return TimeSpan;

            case System.Uri:
                return Uri;

            case not null: // object
                var jsonSchemaPropertyForObject = new JsonSchemaProperty { Type = JsonObjectType.Object };
                foreach (var propertyInfo in value.GetType().GetProperties())
                {
                    var propertyValue = propertyInfo.GetValue(value);
                    var jsonSchemaProperty = value != null ? ConvertValue(propertyValue) : ConvertType(propertyInfo.PropertyType);
                    jsonSchemaPropertyForObject.Properties.Add(propertyInfo.Name, jsonSchemaProperty);
                }

                //var schemaForObject = ToJsonSchema(value);
                //var jsonSchemaProperty = new JsonSchemaProperty { Type = JsonObjectType.Object };
                //foreach (var property in schemaForObject.Properties)
                //{
                //    jsonSchemaProperty.Properties.Add(property.Key, property.Value);
                //}

                return jsonSchemaPropertyForObject;

            case null: // null
                return Null;
        }
    }

    private static JsonSchemaProperty ConvertType(Type type)
    {
        if (type == typeof(bool) || type == typeof(bool?))
        {
            return Boolean;
        }

        if (type == typeof(byte) || type == typeof(byte?))
        {
            return Byte;
        }

        if (type == typeof(DateTime) || type == typeof(DateTime?))
        {
            return Date;
        }

        if (type == typeof(float) || type == typeof(float?))
        {
            return Float;
        }

        if (type == typeof(double) || type == typeof(double?))
        {
            return Double;
        }

        if (type == typeof(Guid) || type == typeof(Guid?))
        {
            return Guid;
        }

        if (type == typeof(int) || type == typeof(short) || type == typeof(int?) || type == typeof(short?))
        {
            return Integer;
        }

        if (type == typeof(long) || type == typeof(long?))
        {
            return Long;
        }

        if (type == typeof(string))
        {
            return String;
        }

        if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
        {
            return TimeSpan;
        }

        if (type == typeof(Uri))
        {
            return Uri;
        }

        return Object;
    }
}