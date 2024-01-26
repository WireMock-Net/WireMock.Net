using WireMock.Admin.Mappings;
using WireMock.Admin.Settings;
using WireMock.Settings;
using ClassMapper;

namespace WireMock.Util;

internal sealed class MapperUtils
{
    public static MapperUtils Instance { get; } = new();

    private MapperUtils()
    {
        Mapper.Bind<ProxyAndRecordSettings, ProxyAndRecordSettingsModel>();
        Mapper.Bind<WebProxySettings, WebProxySettingsModel>();
        Mapper.Bind<WebProxySettings, WebProxyModel>();
        Mapper.Bind<ProxyUrlReplaceSettings, ProxyUrlReplaceSettingsModel>();

        Mapper.Bind<ProxyAndRecordSettingsModel, ProxyAndRecordSettings>();
        Mapper.Bind<WebProxySettingsModel, WebProxySettings>();
        Mapper.Bind<WebProxyModel, WebProxySettings>();
        Mapper.Bind<ProxyUrlReplaceSettingsModel, ProxyUrlReplaceSettings>();
    }

    public ProxyAndRecordSettingsModel? Map(ProxyAndRecordSettings? instance)
    {
        return instance == null ? null : Mapper.Map<ProxyAndRecordSettingsModel>(instance);
    }

    public ProxyAndRecordSettings? Map(ProxyAndRecordSettingsModel? model)
    {
        return model == null ? null : Mapper.Map<ProxyAndRecordSettings>(model);
    }

    public ProxyUrlReplaceSettingsModel? Map(ProxyUrlReplaceSettings? instance)
    {
        return instance == null ? null : Mapper.Map<ProxyUrlReplaceSettingsModel>(instance);
    }

    public ProxyUrlReplaceSettings? Map(ProxyUrlReplaceSettingsModel? model)
    {
        return model == null ? null : Mapper.Map<ProxyUrlReplaceSettings>(model);
    }

    public WebProxyModel? Map(WebProxySettings? instance)
    {
        return instance == null ? null : Mapper.Map<WebProxyModel>(instance);
    }

    public WebProxySettings? Map(WebProxyModel? model)
    {
        return model == null ? null : Mapper.Map<WebProxySettings>(model);
    }
}