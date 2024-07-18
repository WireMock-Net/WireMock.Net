// Copyright © WireMock.Net

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
        var replaceSettings = proxyAndRecordSettings.ReplaceSettings;
        string proxyUrl;
        if (replaceSettings is not null)
        {
            var stringComparison = replaceSettings.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            proxyUrl = url.Replace(replaceSettings.OldValue, replaceSettings.NewValue, stringComparison);
        }
        else
        {
            proxyUrl = url;
        }

        var httpRequestMessage = HttpRequestMessageHelper.Create(requestMessage, proxyUrl);

        // Call the URL
        var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

        // Create ResponseMessage
        bool deserializeJson = !_settings.DisableJsonBodyParsing.GetValueOrDefault(false);
        bool decompressGzipAndDeflate = !_settings.DisableRequestBodyDecompressing.GetValueOrDefault(false);
        bool deserializeFormUrlEncoded = !_settings.DisableDeserializeFormUrlEncoded.GetValueOrDefault(false);

        var responseMessage = await HttpResponseMessageHelper.CreateAsync(
            httpResponseMessage,
            requiredUri,
            originalUri,
            deserializeJson,
            decompressGzipAndDeflate,
            deserializeFormUrlEncoded
        ).ConfigureAwait(false);

        IMapping? newMapping = null;

        var saveMappingSettings = proxyAndRecordSettings.SaveMappingSettings;

        bool save = true;
        if (saveMappingSettings != null)
        {
            save &= Check(saveMappingSettings.StatusCodePattern,
                () => saveMappingSettings.StatusCodePattern != null && HttpStatusRangeParser.IsMatch(saveMappingSettings.StatusCodePattern, responseMessage.StatusCode)
            );

            save &= Check(saveMappingSettings.HttpMethods,
                () => saveMappingSettings.HttpMethods != null && saveMappingSettings.HttpMethods.Value.Contains(requestMessage.Method, StringComparer.OrdinalIgnoreCase)
            );
        }

        if (save && (proxyAndRecordSettings.SaveMapping || proxyAndRecordSettings.SaveMappingToFile))
        {
            newMapping = _proxyMappingConverter.ToMapping(mapping, proxyAndRecordSettings, requestMessage, responseMessage);
        }

        return (responseMessage, newMapping);
    }

    private static bool Check<T>(ProxySaveMappingSetting<T>? saveMappingSetting, Func<bool> action) where T : notnull
    {
        var isMatch = saveMappingSetting is null || action();
        var matchBehaviour = saveMappingSetting?.MatchBehaviour ?? MatchBehaviour.AcceptOnMatch;
        return isMatch == (matchBehaviour == MatchBehaviour.AcceptOnMatch);
    }
}