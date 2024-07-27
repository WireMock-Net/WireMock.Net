// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using AnyOfTypes;
using Newtonsoft.Json;

namespace WireMock.Models;

/// <summary>
/// GraphQLSchemaDetails
/// </summary>
public class GraphQLSchemaDetails
{
    /// <summary>
    /// The GraphQL schema as a string.
    /// </summary>
    public string? SchemaAsString { get; set; }

    /// <summary>
    /// The GraphQL schema as a StringPattern.
    /// </summary>
    public StringPattern? SchemaAsStringPattern { get; set; }

    /// <summary>
    /// The GraphQL schema as a "GraphQL.Types.ISchema".
    /// </summary>
    public object? SchemaAsISchema { get; set; }

    /// <summary>
    /// The GraphQL Schema.
    /// </summary>
    [JsonIgnore]
    public AnyOf<string, StringPattern, object>? Schema
    {
        get
        {
            if (SchemaAsString != null)
            {
                return SchemaAsString;
            }

            if (SchemaAsStringPattern != null)
            {
                return SchemaAsStringPattern;
            }

            if (SchemaAsISchema != null)
            {
                return new AnyOf<string, StringPattern, object>(SchemaAsISchema);
            }

            return null;
        }
    }

    /// <summary>
    /// The custom Scalars to define for this schema.
    /// </summary>
    public IDictionary<string, Type>? CustomScalars { get; set; }
}