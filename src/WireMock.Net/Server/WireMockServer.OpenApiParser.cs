#if OPENAPIPARSER
using System;
using System.Linq;
using System.Net;
using WireMock.Constants;
using WireMock.Net.OpenApiParser;
using WireMock.RequestBuilders;
using WireMock.ResponseProviders;

namespace WireMock.Server;

public partial class WireMockServer
{
    private const string AdminOpenApi = "/__admin/openapi";

    private void InitOpenApiParserAdmin()
    {
        Given(Request.Create().WithPath($"{AdminOpenApi}/convert").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ConvertOpenApiToMappings));

        Given(Request.Create().WithPath($"{AdminOpenApi}/save").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(SaveOpenApiToMappings));

    }

    private IResponseMessage ConvertOpenApiToMappings(IRequestMessage requestMessage)
    {
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
    }

    private IResponseMessage SaveOpenApiToMappings(IRequestMessage requestMessage)
    {
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
    }
}
#endif