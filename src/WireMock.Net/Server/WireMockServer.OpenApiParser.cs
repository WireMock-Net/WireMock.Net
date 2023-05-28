#if OPENAPIPARSER
using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using WireMock.Net.OpenApiParser;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;
using WireMock.Serialization;
#endif

namespace WireMock.Server;

public partial class WireMockServer
{
#if OPENAPIPARSER
    private readonly WireMockOpenApiParserSettings _openApiParserSettings = new WireMockOpenApiParserSettings
    {
        PathPatternToUse = ExampleValueType.Regex,
        HeaderPatternToUse = ExampleValueType.Regex,
        QueryParameterPatternToUse = ExampleValueType.Regex
    };
#endif

    private IResponseMessage OpenApiConvertToMappings(IRequestMessage requestMessage)
    {
#if OPENAPIPARSER
        try
        {
            var mappingModels = new WireMockOpenApiParser().FromText(requestMessage.Body, out var diagnostic);
            if (diagnostic.Errors.Any())
            {
                var diagnosticAsJson = JsonConvert.SerializeObject(diagnostic, JsonSerializationConstants.JsonSerializerSettingsDefault);
                _settings.Logger.Warn("OpenApiError(s) while converting OpenAPI specification to MappingModel(s) : {0}", diagnosticAsJson);
            }

            return ToJson(mappingModels);
        }
        catch (Exception e)
        {
            _settings.Logger.Error("HttpStatusCode set to {0} {1}", HttpStatusCode.BadRequest, e);
            return ResponseMessageBuilder.Create(e.Message, HttpStatusCode.BadRequest);
        }
#else
        return ResponseMessageBuilder.Create("Not supported for .NETStandard 1.3 and .NET 4.5.2 or lower.", 400);
#endif
    }

    private IResponseMessage OpenApiSaveToMappings(IRequestMessage requestMessage)
    {
#if OPENAPIPARSER
        try
        {
            var mappingModels = new WireMockOpenApiParser().FromText(requestMessage.Body, out var diagnostic);
            if (diagnostic.Errors.Any())
            {
                var diagnosticAsJson = JsonConvert.SerializeObject(diagnostic, JsonSerializationConstants.JsonSerializerSettingsDefault);
                _settings.Logger.Warn("OpenApiError(s) while converting OpenAPI specification to MappingModel(s) : {0}", diagnosticAsJson);
            }

            ConvertMappingsAndRegisterAsRespondProvider(mappingModels);

            return ResponseMessageBuilder.Create("OpenApi document converted to Mappings", HttpStatusCode.Created);
        }
        catch (Exception e)
        {
            _settings.Logger.Error("HttpStatusCode set to {0} {1}", HttpStatusCode.BadRequest, e);
            return ResponseMessageBuilder.Create(e.Message, HttpStatusCode.BadRequest);
        }
#else
        return ResponseMessageBuilder.Create("Not supported for .NETStandard 1.3 and .NET 4.5.2 or lower.", 400);
#endif
    }
}