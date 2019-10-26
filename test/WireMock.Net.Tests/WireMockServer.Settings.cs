using Moq;
using NFluent;
using System.Linq;
using WireMock.Logging;
using WireMock.Owin;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class WireMockServerSettingsTests
    {
        private readonly Mock<IWireMockLogger> _loggerMock;

        public WireMockServerSettingsTests()
        {
            _loggerMock = new Mock<IWireMockLogger>();
            _loggerMock.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()));
        }

        [Fact]
        public void WireMockServer_WireMockServerSettings_StartAdminInterfaceTrue_BasicAuthenticationIsSet()
        {
            // Assign and Act
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = true,
                AdminUsername = "u",
                AdminPassword = "p"
            });

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            Check.That(options.AuthorizationMatcher).IsNotNull();
        }

        [Fact]
        public void WireMockServer_WireMockServerSettings_StartAdminInterfaceFalse_BasicAuthenticationIsNotSet()
        {
            // Assign and Act
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = false,
                AdminUsername = "u",
                AdminPassword = "p"
            });

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            Check.That(options.AuthorizationMatcher).IsNull();
        }

        [Fact]
        public void WireMockServer_WireMockServerSettings_PriorityFromAllAdminMappingsIsLow_When_StartAdminInterface_IsTrue()
        {
            // Assign and Act
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = true
            });

            // Assert
            var mappings = server.Mappings;
            Check.That(mappings.Count()).IsEqualTo(24);
            Check.That(mappings.All(m => m.Priority == int.MinValue)).IsTrue();
        }

        [Fact]
        public void WireMockServer_WireMockServerSettings_ProxyAndRecordSettings_ProxyPriority_Is1000_When_StartAdminInterface_IsTrue()
        {
            // Assign and Act
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = true,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "www.google.com"
                }
            });

            // Assert
            var mappings = server.Mappings;
            Check.That(mappings.Count()).IsEqualTo(25);
            Check.That(mappings.Count(m => m.Priority == int.MinValue)).IsEqualTo(24);
            Check.That(mappings.Count(m => m.Priority == 1000)).IsEqualTo(1);
        }

        [Fact]
        public void WireMockServer_WireMockServerSettings_ProxyAndRecordSettings_ProxyPriority_Is0_When_StartAdminInterface_IsFalse()
        {
            // Assign and Act
            var server = WireMockServer.Start(new WireMockServerSettings
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

        [Fact]
        public void WireMockServer_WireMockServerSettings_AllowPartialMapping()
        {
            // Assign and Act
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Logger = _loggerMock.Object,
                AllowPartialMapping = true
            });

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            Check.That(options.AllowPartialMapping).Equals(true);

            // Verify
            _loggerMock.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<bool>()));
        }

        [Fact]
        public void WireMockServer_WireMockServerSettings_AllowBodyForAllHttpMethods()
        {
            // Assign and Act
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Logger = _loggerMock.Object,
                AllowBodyForAllHttpMethods = true
            });

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            Check.That(options.AllowBodyForAllHttpMethods).Equals(true);

            // Verify
            _loggerMock.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<bool>()));
        }

        [Fact]
        public void WireMockServer_WireMockServerSettings_RequestLogExpirationDuration()
        {
            // Assign and Act
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Logger = _loggerMock.Object,
                RequestLogExpirationDuration = 1
            });

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            Check.That(options.RequestLogExpirationDuration).IsEqualTo(1);
        }
    }
}