using System;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using WireMock.Server;

namespace WireMock.Net.OpenApiParser.Extensions
{
    public static class WireMockServerExtensions
    {
        /// <summary>
        /// Register the mappings via an OpenAPI (swagger) V2 or V3 file.
        /// </summary>
        /// <param name="server">The WireMockServer instance</param>
        /// <param name="path">Path containing OpenAPI file to parse and use the mappings.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        public static IWireMockServer WithMappingFromOpenApiFile(this IWireMockServer server, string path, out OpenApiDiagnostic diagnostic)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var mappings = new WireMockOpenApiParser().FromFile(path, out diagnostic);

            return server.WithMapping(mappings.ToArray());
        }

        /// <summary>
        /// Register the mappings via an OpenAPI (swagger) V2 or V3 stream.
        /// </summary>
        /// <param name="server">The WireMockServer instance</param>
        /// <param name="stream">Stream containing OpenAPI description to parse and use the mappings.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        public static IWireMockServer WithMappingFromOpenApiStream(this IWireMockServer server, Stream stream, out OpenApiDiagnostic diagnostic)
        {
            var mappings = new WireMockOpenApiParser().FromStream(stream, out diagnostic);

            return server.WithMapping(mappings.ToArray());
        }

        /// <summary>
        /// Register the mappings via an OpenAPI (swagger) V2 or V3 document.
        /// </summary>
        /// <param name="server">The WireMockServer instance</param>
        /// <param name="document">The OpenAPI document to use as mappings.</param>
        public static IWireMockServer WithMappingFromOpenApiDocument(this IWireMockServer server, OpenApiDocument document)
        {
            var mappings = new WireMockOpenApiParser().FromDocument(document);

            return server.WithMapping(mappings.ToArray());
        }
    }
}