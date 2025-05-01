// Copyright © WireMock.Net

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace WireMock.Net.Tests.Authentication;

internal class MockJwtSecurityTokenHandler : JwtSecurityTokenHandler
{
    public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        validatedToken = new JwtSecurityToken();
        return ClaimsPrincipal.Current;    
    }
}