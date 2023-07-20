using AgileObjects.AgileMapper;
using WireMock.Admin.Settings;
using WireMock.Settings;

namespace WireMock.Util;

internal sealed class AgileMapperUtils
{
    public static AgileMapperUtils Instance { get; } = new();

    private AgileMapperUtils()
    {
        Mapper.WhenMapping.From<ProxyAndRecordSettings>().To<ProxyAndRecordSettingsModel>();
        Mapper.WhenMapping.From<WebProxySettings>().To<WebProxySettingsModel>();
        Mapper.WhenMapping.From<ProxyUrlReplaceSettings>().To<ProxyUrlReplaceSettingsModel>();

        Mapper.WhenMapping.From<ProxyAndRecordSettingsModel>().To<ProxyAndRecordSettings>();
        Mapper.WhenMapping.From<WebProxySettingsModel>().To<WebProxySettings>();
        Mapper.WhenMapping.From<ProxyUrlReplaceSettingsModel>().To<ProxyUrlReplaceSettings>();
    }

    public ProxyAndRecordSettingsModel? Map(ProxyAndRecordSettings? instance)
    {
        return instance == null ? null : Mapper.Map(instance).ToANew<ProxyAndRecordSettingsModel>();
    }

    public ProxyAndRecordSettings? Map(ProxyAndRecordSettingsModel? model)
    {
        return model == null ? null : Mapper.Map(model).ToANew<ProxyAndRecordSettings>();
    }
}