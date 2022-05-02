using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace WireMock.NSwagExtensions;

internal static class NSwagSchemaExtensions
{
    public static JsonSchema ToJsonSchema(this JObject instance)
    {
        static JsonSchemaProperty ConvertJToken(JToken value)
        {
            var type = value.Type;
            switch (type)
            {
                case JTokenType.Array:
                    return new JsonSchemaProperty
                    {
                        Type = JsonObjectType.Array,
                        Items =
                        {
                            new JsonSchemaProperty
                            {
                                Type = value.HasValues
                                    ? ConvertJToken(value.First!).Type
                                    : JsonObjectType.Object
                            }
                        }
                    };

                case JTokenType.Boolean:
                    return new JsonSchemaProperty { Type = JsonObjectType.Boolean };

                case JTokenType.Bytes:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.Byte };

                case JTokenType.Date:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.DateTime };

                case JTokenType.Guid:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.Guid };

                case JTokenType.Float:
                    return new JsonSchemaProperty { Type = JsonObjectType.Number, Format = JsonFormatStrings.Float };

                case JTokenType.Integer:
                    return new JsonSchemaProperty { Type = JsonObjectType.Integer, Format = JsonFormatStrings.Integer };

                case JTokenType.Null:
                    return new JsonSchemaProperty { Type = JsonObjectType.Null };

                case JTokenType.Object:
                    var schemaForObject = ToJsonSchema((JObject)value);
                    var jsonSchemaProperty = new JsonSchemaProperty { Type = JsonObjectType.Object };
                    foreach (var property in schemaForObject.Properties)
                    {
                        jsonSchemaProperty.Properties.Add(property.Key, property.Value);
                    }

                    return jsonSchemaProperty;

                case JTokenType.String:
                    return new JsonSchemaProperty { Type = JsonObjectType.String };

                case JTokenType.TimeSpan:

                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.TimeSpan };

                case JTokenType.Uri:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.Uri };

                default:
                    return new JsonSchemaProperty { Type = JsonObjectType.Object };
            }
        }

        var schema = new JsonSchema();
        foreach (var property in instance.Properties())
        {
            schema.Properties.Add(property.Name, ConvertJToken(property.Value));
        }

        return schema;
    }

    public static JsonSchema ToJsonSchema(this object instance)
    {
        static JsonSchemaProperty ConvertValue(object value)
        {
            switch (value)
            {
                case IEnumerable<byte>:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.Byte };

                //case Array enumerable:
                //    return new JsonSchemaProperty
                //    {
                //        Type = JsonObjectType.Array,
                //        Items =
                //        {
                //            new JsonSchemaProperty
                //            {
                //                Type = enumerable.Length > 0
                //                    ? ConvertJToken(enumerable[0]).Type
                //                    : JsonObjectType.Object
                //            }
                //        }
                //    };

                case bool:
                    return new JsonSchemaProperty { Type = JsonObjectType.Boolean };

                case DateTime:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.DateTime };

                case Guid:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.Guid };

                case double:
                case float:
                    return new JsonSchemaProperty { Type = JsonObjectType.Number, Format = JsonFormatStrings.Float };

                case short:
                case int:
                    return new JsonSchemaProperty { Type = JsonObjectType.Integer, Format = JsonFormatStrings.Integer };

                case long:
                    return new JsonSchemaProperty { Type = JsonObjectType.Integer, Format = JsonFormatStrings.Long };

                case string:
                    return new JsonSchemaProperty { Type = JsonObjectType.String };

                case TimeSpan:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.TimeSpan };

                case Uri:
                    return new JsonSchemaProperty { Type = JsonObjectType.String, Format = JsonFormatStrings.Uri };

                case not null: // object
                    var schemaForObject = ToJsonSchema(value);
                    var jsonSchemaProperty = new JsonSchemaProperty { Type = JsonObjectType.Object };
                    foreach (var property in schemaForObject.Properties)
                    {
                        jsonSchemaProperty.Properties.Add(property.Key, property.Value);
                    }

                    return jsonSchemaProperty;

                default:
                    return new JsonSchemaProperty { Type = JsonObjectType.Object };
            }
        }

        var schema = new JsonSchema();
        foreach (var item in instance.GetType().GetProperties())
        {
            schema.Properties.Add(item.Name, ConvertValue(item.GetValue(instance)));
        }

        return schema;
    }
}