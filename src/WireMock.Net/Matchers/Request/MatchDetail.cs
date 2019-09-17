using System;

namespace WireMock.Matchers.Request
{
    public class MatchDetail
    {
        public Type MatcherType { get; set; }

        public double Score { get; set; }
    }
}