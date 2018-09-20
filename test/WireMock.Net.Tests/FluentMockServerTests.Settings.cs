using System.Linq;
using NFluent;
using WireMock.Owin;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerSettingsTests
    {
        [Fact]
        public void FluentMockServer_FluentMockServerSettings_StartAdminInterfaceTrue_BasicAuthenticationIsSet()
        {
            // Assign and Act
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                StartAdminInterface = true,
                AdminUsername = "u",
                AdminPassword = "p"
            });

            // Assert
            var options = server.GetPrivateFieldValue<WireMockMiddlewareOptions>("_options");
            Check.That(options.AuthorizationMatcher).IsNotNull();
        }

        [Fact]
        public void FluentMockServer_FluentMockServerSettings_StartAdminInterfaceFalse_BasicAuthenticationIsNotSet()
        {
            // Assign and Act
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                StartAdminInterface = false,
                AdminUsername = "u",
                AdminPassword = "p"
            });

            // Assert
            var options = server.GetPrivateFieldValue<WireMockMiddlewareOptions>("_options");
            Check.That(options.AuthorizationMatcher).IsNull();
        }

        [Fact]
        public void FluentMockServer_FluentMockServerSettings_PriorityFromAllAdminMappingsIsLow_When_StartAdminInterface_IsTrue()
        {
            // Assign and Act
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                StartAdminInterface = true
            });

            // Assert
            var mappings = server.Mappings;
            Check.That(mappings.Count()).IsEqualTo(19);
            Check.That(mappings.All(m => m.Priority == int.MinValue)).IsTrue();
        }

        [Fact]
        public void FluentMockServer_FluentMockServerSettings_ProxyAndRecordSettings_ProxyPriority_Is1000_When_StartAdminInterface_IsTrue()
        {
            // Assign and Act
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                StartAdminInterface = true,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "www.google.com"
                }
            });

            // Assert
            var mappings = server.Mappings;
            Check.That(mappings.Count()).IsEqualTo(20);
            Check.That(mappings.Count(m => m.Priority == int.MinValue)).IsEqualTo(19);
            Check.That(mappings.Count(m => m.Priority == 1000)).IsEqualTo(1);
        }

        [Fact]
        public void FluentMockServer_FluentMockServerSettings_ProxyAndRecordSettings_ProxyPriority_Is0_When_StartAdminInterface_IsFalse()
        {
            // Assign and Act
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "www.google.com"
                }
            });

            // Assert
            var mappings = server.Mappings.ToArray();
            Check.That(mappings.Count()).IsEqualTo(1);
            Check.That(mappings[0].Priority).IsEqualTo(0);
        }
    }
}