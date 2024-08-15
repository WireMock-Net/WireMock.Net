// Copyright © WireMock.Net

using Riok.Mapperly.Abstractions;
using WireMock.Admin.Mappings;
using WireMock.Admin.Settings;
using WireMock.Settings;

namespace WireMock.Util;

[Mapper]
internal static partial class TinyMapperUtils
{
    public static partial ProxyAndRecordSettingsModel? Map(ProxyAndRecordSettings? instance);

    public static partial ProxyAndRecordSettings? Map(ProxyAndRecordSettingsModel? model);

    public static partial ProxyUrlReplaceSettingsModel? Map(ProxyUrlReplaceSettings? instance);

    public static partial ProxyUrlReplaceSettings? Map(ProxyUrlReplaceSettingsModel? model);

    public static partial WebProxyModel? Map(WebProxySettings? instance);

    public static partial WebProxySettings? Map(WebProxyModel? model);
}