using NFluent;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerContentTypeTests
    {
        [Fact]
        public async Task FluentMockServer_Should_accept_content_type_with_no_charset()
        {
            // Assign
            var server = FluentMockServer.StartWithAdminInterface();
            
            // Act
            var uri = $"{server.Urls[0]}/__admin/mappings";

            var message = @"{
                ""request"": {
                    ""method"": ""GET"",
                    ""url"": ""/some/thing""
                },
                ""response"": {
                    ""status"": 200,
                    ""body"": ""Hello world!"",
                    ""headers"": {
                        ""Content-Type"": ""text/plain""
                    }
                }
            }";
            var content = new StringContent(message, Encoding.UTF8, "application/json");
            content.Headers.ContentType.CharSet = "";
            HttpResponseMessage resp = await new HttpClient().PostAsync(uri, content);

            // Assert
            Check.That(resp.StatusCode).Equals(HttpStatusCode.Created);
        }
 
        [Fact]
        public async Task FluentMockServer_Should_accept_content_type_with_charset()
        {
            // Assign
            var server = FluentMockServer.StartWithAdminInterface();
            
            // Act
            var uri = $"{server.Urls[0]}/__admin/mappings";

            var message = @"{
                ""request"": {
                    ""method"": ""GET"",
                    ""url"": ""/some/thing""
                },
                ""response"": {
                    ""status"": 200,
                    ""body"": ""Hello world!"",
                    ""headers"": {
                        ""Content-Type"": ""text/plain""
                    }
                }
            }";
            var content = new StringContent(message, Encoding.UTF8, "application/json");
            HttpResponseMessage resp = await new HttpClient().PostAsync(uri, content);

            // Assert
            Check.That(resp.StatusCode).Equals(HttpStatusCode.Created);
        }
    }       
}