// Copyright © WireMock.Net

using System;

namespace WireMock.Extensions;

internal static class ExceptionExtensions
{
    public static Exception? ToException(this Exception[] exceptions)
    {
        return exceptions.Length switch
        {
            1 => exceptions[0],
            > 1 => new AggregateException(exceptions),
            _ => null
        };
    }
}