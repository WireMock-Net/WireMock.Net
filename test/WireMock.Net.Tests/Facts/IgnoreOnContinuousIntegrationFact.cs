// Copyright © WireMock.Net
using System;
using Xunit;

namespace WireMock.Net.Tests.Facts
{
    public sealed class IgnoreOnContinuousIntegrationFact : FactAttribute
    {
        public IgnoreOnContinuousIntegrationFact()
        {
            if (IsContinuousIntegration())
            {
                Skip = "Ignore when run via CI/CD";
            }
        }

        private static bool IsContinuousIntegration() => Environment.GetEnvironmentVariable("TF_BUILD") != null && Environment.GetEnvironmentVariable("TF_BUILD").ToLower() == "true";
    }
}
