using System.Linq;
using NFluent;
using WireMock.Matchers;
using WireMock.Owin;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerAuthenticationTests
    {
        [Fact]
        public void FluentMockServer_Authentication_SetBasicAuthentication()
        {
            // Assign
            var server = FluentMockServer.Start();

            // Act
            server.SetBasicAuthentication("x", "y");

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            Check.That(options.AuthorizationMatcher.Name).IsEqualTo("RegexMatcher");
            Check.That(options.AuthorizationMatcher.MatchBehaviour).IsEqualTo(MatchBehaviour.AcceptOnMatch);
            Check.That(options.AuthorizationMatcher.GetPatterns()).ContainsExactly("^(?i)BASIC eDp5$");
        }

        [Fact]
        public void FluentMockServer_Authentication_RemoveBasicAuthentication()
        {
            // Assign
            var server = FluentMockServer.Start();
            server.SetBasicAuthentication("x", "y");

            // Act
            server.RemoveBasicAuthentication();

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            Check.That(options.AuthorizationMatcher).IsNull();
        }
    }
}