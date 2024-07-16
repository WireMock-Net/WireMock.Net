// Copyright © WireMock.Net

// Copied from https://github.com/Handlebars-Net/Handlebars.Net.Helpers/blob/master/src/Handlebars.Net.Helpers.DynamicLinq

using System.Linq.Dynamic.Core;

namespace WireMock.Json;

internal class DynamicPropertyWithValue : DynamicProperty
{
    public object? Value { get; }

    public DynamicPropertyWithValue(string name, object? value) : base(name, value?.GetType() ?? typeof(object))
    {
        Value = value;
    }
}