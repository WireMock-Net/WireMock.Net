using System.Collections.Generic;

namespace WireMock.Pact.Models.V2
{
    public class Response
    {
        public object Body { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public int Status { get; set; } = 200;
    }
}