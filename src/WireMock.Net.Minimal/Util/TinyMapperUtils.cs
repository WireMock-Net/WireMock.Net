// Copyright © WireMock.Net

using Nelibur.ObjectMapper;
using WireMock.Admin.Mappings;
using WireMock.Admin.Settings;
using WireMock.Settings;

namespace WireMock.Util;

internal sealed class TinyMapperUtils
{
    public static TinyMapperUtils Instance { get; } = new();

    private TinyMapperUtils()
    {
        TinyMapper.Bind<ProxyAndRecordSettings, ProxyAndRecordSettingsModel>();
        TinyMapper.Bind<WebProxySettings, WebProxySettingsModel>();
        TinyMapper.Bind<WebProxySettings, WebProxyModel>();
        TinyMapper.Bind<ProxyUrlReplaceSettings, ProxyUrlReplaceSettingsModel>();

        TinyMapper.Bind<ProxyAndRecordSettingsModel, ProxyAndRecordSettings>();
        TinyMapper.Bind<WebProxySettingsModel, WebProxySettings>();
        TinyMapper.Bind<WebProxyModel, WebProxySettings>();
        TinyMapper.Bind<ProxyUrlReplaceSettingsModel, ProxyUrlReplaceSettings>();
    }

    public ProxyAndRecordSettingsModel? Map(ProxyAndRecordSettings? instance)
    {
        return instance == null ? null : TinyMapper.Map<ProxyAndRecordSettingsModel>(instance);
    }

    public ProxyAndRecordSettings? Map(ProxyAndRecordSettingsModel? model)
    {
        return model == null ? null : TinyMapper.Map<ProxyAndRecordSettings>(model);
    }

    public ProxyUrlReplaceSettingsModel? Map(ProxyUrlReplaceSettings? instance)
    {
        return instance == null ? null : TinyMapper.Map<ProxyUrlReplaceSettingsModel>(instance);
    }

    public ProxyUrlReplaceSettings? Map(ProxyUrlReplaceSettingsModel? model)
    {
        return model == null ? null : TinyMapper.Map<ProxyUrlReplaceSettings>(model);
    }

    public WebProxyModel? Map(WebProxySettings? instance)
    {
        return instance == null ? null : TinyMapper.Map<WebProxyModel>(instance);
    }

    public WebProxySettings? Map(WebProxyModel? model)
    {
        return model == null ? null : TinyMapper.Map<WebProxySettings>(model);
    }
}