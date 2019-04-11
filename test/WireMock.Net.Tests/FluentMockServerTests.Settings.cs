using System.Linq;
using Moq;
using NFluent;
using WireMock.Logging;
using WireMock.Owin;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerSettingsTests
    {
        private readonly Mock<IWireMockLogger> _loggerMock;

        public FluentMockServerSettingsTests()
        {
            _loggerMock = new Mock<IWireMockLogger>();
            _loggerMock.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()));
        }

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
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
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
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
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
            Check.That(mappings.Count()).IsEqualTo(24);
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
            Check.That(mappings.Count()).IsEqualTo(25);
            Check.That(mappings.Count(m => m.Priority == int.MinValue)).IsEqualTo(24);
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

        [Fact]
        public void FluentMockServer_FluentMockServerSettings_AllowPartialMapping()
        {
            // Assign and Act
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Logger = _loggerMock.Object,
                AllowPartialMapping = true
            });

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            Check.That(options.AllowPartialMapping).IsTrue();

            // Verify
            _loggerMock.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<bool>()));
        }

        [Fact]
        public void FluentMockServer_FluentMockServerSettings_RequestLogExpirationDuration()
        {
            // Assign and Act
            var server = FluentMockServer.Start(new FluentMockServerSettings
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