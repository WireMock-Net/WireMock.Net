using NFluent;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerSettingsTests
    {
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