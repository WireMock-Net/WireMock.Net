using System;
using FluentAssertions;
using WireMock.Admin.Mappings;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests.WithMapping
{
    public class WireMockServerTests
    {
        [Fact]
        public void WireMockServer_WithMapping_Should_Add_Mapping()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var pattern = "hello wiremock";
            var path = "/foo";
            var response = "OK";
            var mapping = new MappingModel
            {
                Guid = guid,
                Request = new RequestModel
                {
                    Path = path,
                    Body = new BodyModel
                    {
                        Matcher = new MatcherModel
                        {
                            Name = "ExactMatcher",
                            Pattern = pattern
                        }
                    }
                },
                Response = new ResponseModel
                {
                    Body = response
                }
            };

            var server = WireMockServer.Start();

            // Act
            server.WithMapping(mapping);

            // Assert
            server.MappingModels.Should().HaveCount(1).And.Contain(m =>
                m.Guid == guid &&
                //((PathModel)m.Request.Path).Matchers.OfType<WildcardMatcher>().First().GetPatterns().First() == "/foo*"
                // m.Request.Body.Matchers.OfType<ExactMatcher>().First().GetPatterns().First() == pattern &&
                m.Response.Body == response
            );
        }
    }
}
