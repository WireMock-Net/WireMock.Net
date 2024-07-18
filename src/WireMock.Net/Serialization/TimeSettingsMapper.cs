// Copyright © WireMock.Net

using WireMock.Models;

namespace WireMock.Serialization;

internal static class TimeSettingsMapper
{
    public static TimeSettingsModel? Map(ITimeSettings? settings)
    {
        return settings != null ? new TimeSettingsModel
        {
            Start = settings.Start,
            End = settings.End,
            TTL = settings.TTL
        } : null;
    }

    public static ITimeSettings? Map(TimeSettingsModel? settings)
    {
        return settings != null ? new TimeSettings
        {
            Start = settings.Start,
            End = settings.End,
            TTL = settings.TTL
        } : null;
    }
}