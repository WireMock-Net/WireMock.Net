#if !NETSTANDARD1_3
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using AnyOfTypes;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Authentication
{
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
            _audience = audience;
            _stsDiscoveryEndpoint = string.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/.well-known/openid-configuration", tenant);
        }

        public string Name => nameof(AzureADAuthenticationMatcher);

        public MatchBehaviour MatchBehaviour => MatchBehaviour.AcceptOnMatch;

        public bool ThrowException => false;

        public AnyOf<string, StringPattern>[] GetPatterns()
        {
            return new AnyOf<string, StringPattern>[0];
        }

        public double IsMatch(string input)
        {
            var token = Regex.Replace(input, BearerPrefix, string.Empty, RegexOptions.IgnoreCase);

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
            catch
            {
                return MatchScores.Mismatch;
            }
        }
    }
}
#endif