using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Readers;
using WireMock.Admin.Mappings;

namespace WireMock.Net.OpenApiParser
{
    public interface IWireMockOpenApiParser
    {
        IEnumerable<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic);
    }
}