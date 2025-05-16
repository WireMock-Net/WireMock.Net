// Copyright © WireMock.Net

using WireMock.Types;

// ReSharper disable once CheckNamespace
namespace WireMock.Util;

// ReSharper disable once InconsistentNaming
public static class IBodyDataExtensions
{
    public static BodyType GetDetectedBodyType(this IBodyData bodyData)
    {
        return bodyData.DetectedBodyType ?? BodyType.None;
    }
}