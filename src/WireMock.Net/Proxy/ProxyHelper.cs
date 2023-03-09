using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock.Proxy;

internal class ProxyHelper
{
    private readonly WireMockServerSettings _settings;
    private readonly ProxyMappingConverter _proxyMappingConverter;

    public ProxyHelper(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
        _proxyMappingConverter = new ProxyMappingConverter(settings, new GuidUtils(), new DateTimeUtils());
    }

    public async Task<(IResponseMessage Message, IMapping? Mapping)> SendAsync(
        IMapping? mapping,
        ProxyAndRecordSettings proxyAndRecordSettings,
        HttpClient client,
        IRequestMessage requestMessage,
        string url)
    {
        Guard.NotNull(client);
        Guard.NotNull(requestMessage);
        Guard.NotNull(url);

        var originalUri = new Uri(requestMessage.Url);
        var requiredUri = new Uri(url);

        // Create HttpRequestMessage
        var httpRequestMessage = HttpRequestMessageHelper.Create(requestMessage, url);

        // Call the URL
        var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

        // Create ResponseMessage
        bool deserializeJson = !_settings.DisableJsonBodyParsing.GetValueOrDefault(false);
        bool decompressGzipAndDeflate = !_settings.DisableRequestBodyDecompressing.GetValueOrDefault(false);

        var responseMessage = await HttpResponseMessageHelper.CreateAsync(httpResponseMessage, requiredUri, originalUri, deserializeJson, decompressGzipAndDeflate).ConfigureAwait(false);

        IMapping? newMapping = null;

        var saveMappingSettings = proxyAndRecordSettings.SaveMappingSettings;

        bool save = true;
        if (saveMappingSettings != null)
        {
            save &= Check(saveMappingSettings.StatusCodePattern,
                () => HttpStatusRangeParser.IsMatch(saveMappingSettings.StatusCodePattern, responseMessage.StatusCode)
            );

            save &= Check(saveMappingSettings.HttpMethods,
                () => saveMappingSettings.HttpMethods.Value.Contains(requestMessage.Method, StringComparer.OrdinalIgnoreCase)
            );
        }

        if (save && (proxyAndRecordSettings.SaveMapping || proxyAndRecordSettings.SaveMappingToFile))
        {
            newMapping = _proxyMappingConverter.ToMapping(mapping, proxyAndRecordSettings, requestMessage, responseMessage);
        }

        return (responseMessage, newMapping);
    }

    private static bool Check<T>(ProxySaveMappingSetting<T>? saveMappingSetting, Func<bool> action)
    {
        var isMatch = saveMappingSetting is null || action();
        var matchBehaviour = saveMappingSetting?.MatchBehaviour ?? MatchBehaviour.AcceptOnMatch;
        return isMatch == (matchBehaviour == MatchBehaviour.AcceptOnMatch);
    }
}