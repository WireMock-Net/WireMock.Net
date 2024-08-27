// Copyright © WireMock.Net

#if !NETSTANDARD1_3
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using AnyOfTypes;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Stef.Validation;
using WireMock.Constants;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Authentication;

/// <summary>
/// https://www.c-sharpcorner.com/article/how-to-validate-azure-ad-token-using-console-application/
/// https://stackoverflow.com/questions/38684865/validation-of-an-azure-ad-bearer-token-in-a-console-application
/// </summary>
internal class AzureADAuthenticationMatcher : IStringMatcher
{
    private const string BearerPrefix = "Bearer ";

    private readonly string _audience;
    private readonly string _stsDiscoveryEndpoint;

    public AzureADAuthenticationMatcher(string tenant, string audience)
    {
        _audience = Guard.NotNullOrEmpty(audience);
        _stsDiscoveryEndpoint = string.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/.well-known/openid-configuration", Guard.NotNullOrEmpty(tenant));
    }

    public string Name => nameof(AzureADAuthenticationMatcher);

    public MatchBehaviour MatchBehaviour => MatchBehaviour.AcceptOnMatch;

    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return EmptyArray<AnyOf<string, StringPattern>>.Value;
    }

    public MatchOperator MatchOperator => MatchOperator.Or;

    public MatchResult IsMatch(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return MatchScores.Mismatch;
        }

        var token = Regex.Replace(input, BearerPrefix, string.Empty, RegexOptions.IgnoreCase, WireMockConstants.DefaultRegexTimeout);

        try
        {
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(_stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
            var config = configManager.GetConfigurationAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = _audience,
                ValidIssuer = config.Issuer,
                IssuerSigningKeys = config.SigningKeys,
                ValidateLifetime = true
            };

            // Throws an Exception as the token is invalid (expired, invalid-formatted, etc.)
            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var _);

            return MatchScores.Perfect;
        }
        catch (Exception ex)
        {
            return new MatchResult(MatchScores.Mismatch, ex);
        }
    }

    /// <inheritdoc />
    public virtual string GetCSharpCodeArguments()
    {
        throw new NotImplementedException();
    }
}
#endif