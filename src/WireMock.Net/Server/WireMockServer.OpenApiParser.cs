#if OPENAPIPARSER
using System;
using System.Linq;
using System.Net;
using WireMock.Net.OpenApiParser;
#endif

namespace WireMock.Server;

public partial class WireMockServer
{
    private IResponseMessage ConvertOpenApiToMappings(IRequestMessage requestMessage)
    {
#if OPENAPIPARSER
        try
        {
            var mappingModels = new WireMockOpenApiParser().FromText(requestMessage.Body, out var diagnostic);
            return diagnostic.Errors.Any() ? ToJson(diagnostic, false, HttpStatusCode.BadRequest) : ToJson(mappingModels);
        }
        catch (Exception e)
        {
            _settings.Logger.Error("HttpStatusCode set to {0} {1}", HttpStatusCode.BadRequest, e);
            return ResponseMessageBuilder.Create(e.ToString(), HttpStatusCode.BadRequest);
        }
#else
        return ResponseMessageBuilder.Create("Not supported for .NETStandard 1.3 and .NET 4.5.2 or lower.", 400);
#endif
    }

    private IResponseMessage SaveOpenApiToMappings(IRequestMessage requestMessage)
    {
#if OPENAPIPARSER
        try
        {
            var mappingModels = new WireMockOpenApiParser().FromText(requestMessage.Body, out var diagnostic);
            if (diagnostic.Errors.Any())
            {
                return ToJson(diagnostic, false, HttpStatusCode.BadRequest);
            }

            ConvertMappingsAndRegisterAsRespondProvider(mappingModels);

            return ResponseMessageBuilder.Create("OpenApi document converted to Mappings", HttpStatusCode.Created);
        }
        catch (Exception e)
        {
            _settings.Logger.Error("HttpStatusCode set to {0} {1}", HttpStatusCode.BadRequest, e);
            return ResponseMessageBuilder.Create(e.ToString(), HttpStatusCode.BadRequest);
        }
#else
        return ResponseMessageBuilder.Create("Not supported for .NETStandard 1.3 and .NET 4.5.2 or lower.", 400);
#endif
    }
}