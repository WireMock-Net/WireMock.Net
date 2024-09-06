// Copyright © WireMock.Net

using System;
using Xunit;

namespace WireMock.Net.Tests.Facts;

public sealed class IgnoreOnContinuousIntegrationFact : FactAttribute
{
    private static readonly string _skipReason = "Ignore when run via CI/CD";
    private static readonly bool _isContinuousIntegrationAzure = bool.TryParse(Environment.GetEnvironmentVariable("TF_BUILD"), out var isTF) && isTF;
    private static readonly bool _isContinuousIntegrationGithub = bool.TryParse(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), out var isGH) && isGH;
    private static bool IsContinuousIntegration() => _isContinuousIntegrationAzure || _isContinuousIntegrationGithub;
    
    public IgnoreOnContinuousIntegrationFact()
    {
        if (IsContinuousIntegration())
        {
            Skip = _skipReason;
        }
    }
}
