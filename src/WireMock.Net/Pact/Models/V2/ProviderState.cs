using System.Collections.Generic;

namespace WireMock.Pact.Models.V2
{
    public class ProviderState
    {
        public string Name { get; set; }

        public IDictionary<string, string> Params { get; set; }
    }
}