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

#if GRAPHQL
    /// <summary>
    /// The GraphQL schema as a <seealso cref="GraphQL.Types.ISchema"/>.
    /// </summary>
    public GraphQL.Types.ISchema? SchemaAsISchema { get; set; }

    /// <summary>
    /// The GraphQL Schema.
    /// </summary>
    [JsonIgnore]
    public AnyOf<string, StringPattern, GraphQL.Types.ISchema>? Schema
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
                return new AnyOf<string, StringPattern, GraphQL.Types.ISchema>(SchemaAsISchema);
            }

            return null;
        }
    }
#endif

    /// <summary>
    /// The custom Scalars to define for this schema.
    /// </summary>
    public IDictionary<string, Type>? CustomScalars { get; set; }
}