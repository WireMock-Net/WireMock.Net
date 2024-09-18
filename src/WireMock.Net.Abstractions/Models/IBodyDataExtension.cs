using WireMock.Types;

// ReSharper disable once CheckNamespace
namespace WireMock.Util;

public static class IBodyDataExtension
{
    public static BodyType GetBodyType(this IBodyData bodyData) {
        if (bodyData.DetectedBodyTypeFromContentType is not null and not BodyType.None) {
            return bodyData.DetectedBodyTypeFromContentType.Value;
        }
        if (bodyData.DetectedBodyType is not null and not BodyType.None) {
            return bodyData.DetectedBodyType.Value;
        }
        return BodyType.None;
    }
}