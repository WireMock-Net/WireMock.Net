using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using WireMock.Admin.Mappings;

namespace WireMock.Net.OpenApiParser
{
    public interface IWireMockOpenApiParser
    {
        IEnumerable<MappingModel> FromDocument(OpenApiDocument document);

        IEnumerable<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic);

        IEnumerable<MappingModel> FromFile(string path, out OpenApiDiagnostic diagnostic);
    }
}