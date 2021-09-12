using System.IO;
using WireMock.Server;
using Xunit;

namespace WireMock.Net.Tests
{
    public class WireMockServerImportWireMockOrgTests
    {
        // For for AppVeyor + OpenCover
        private string GetCurrentFolder()
        {
            string current = Directory.GetCurrentDirectory();
            //if (!current.EndsWith("WireMock.Net.Tests"))
            //    return Path.Combine(current, "test", "WireMock.Net.Tests");

            return current;
        }

        [Fact]
        public void WireMockServer_ImportWireMockOrg()
        {
            var server = WireMockServer.Start();
            string path = Path.Combine(GetCurrentFolder(), "__admin", "mappings.org", "mapping1.json");
            server.ReadStaticWireMockOrgMapping(path);

            server.Stop();
        }
    }
}