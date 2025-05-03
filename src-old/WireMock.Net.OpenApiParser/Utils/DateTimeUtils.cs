// Copyright © WireMock.Net

using System;
using System.Globalization;

namespace WireMock.Net.OpenApiParser.Utils;

internal static class DateTimeUtils
{
    public static string ToRfc3339DateTime(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
    }

    public static string ToRfc3339Date(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
    }
}