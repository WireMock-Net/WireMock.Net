using System.Linq;
using System.Threading.Tasks;
using NFluent;
using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class ClientTests
    {
        [Fact]
        public async Task Client_IFluentMockServerAdmin_PostMappingAsync()
        {
            // Assign
            var server = FluentMockServer.StartWithAdminInterface();
            var api = RestClient.For<IFluentMockServerAdmin>(server.Urls[0]);

            // Act
            var model = new MappingModel
            {
                Request = new RequestModel
                {
                    Path = "/1"
                },
                Response = new ResponseModel
                {
                    Body = "txt",
                    StatusCode = 200
                },
                Priority = 500,
                Title = "test"
            };
            var result = await api.PostMappingAsync(model);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsNotNull();
            Check.That(result.Guid).IsNotNull();

            var mapping = server.Mappings.Single(m => m.Priority == 500);
            Check.That(mapping).IsNotNull();
            Check.That(mapping.Title).Equals("test");

            server.Stop();
        }
    }
}