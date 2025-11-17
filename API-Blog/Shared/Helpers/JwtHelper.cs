using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;

namespace Shared.Helpers;

public class JwtHelper
{
    public static string? GenerateToken(IEnumerable<string> roles, JwtSetting jwtSetting, JwtUserInformation user, DateTime expireStart)
    {
        if (string.IsNullOrEmpty(jwtSetting.SecretKey)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.SecretKey)), SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtSetting.ValidIssuer,
            Audience = jwtSetting.ValidAudience,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name!),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UniqueName!),
                new Claim(ClaimTypes.Email, user.Email!),
            }),
            Expires = expireStart.AddMinutes(jwtSetting.TokenValidityInMinutes),
            SigningCredentials = credentials
        };
        foreach (var item in roles)
        {
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, item ?? ""));
        }

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string? VerifyToken(JwtSetting jwtSetting, string? token)
    {
        var secretKey = jwtSetting.SecretKey;
        var validIssuer = jwtSetting.ValidIssuer;
        var validAudience = jwtSetting.ValidAudience;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters();
        validationParameters.ValidIssuer = validIssuer;
        validationParameters.ValidAudience = validAudience;
        validationParameters.IssuerSigningKey = key;
        validationParameters.ValidateLifetime = false;
        validationParameters.ValidateIssuerSigningKey = true;
        validationParameters.ValidateIssuer = !string.IsNullOrEmpty(validIssuer);
        validationParameters.ValidateAudience = !string.IsNullOrEmpty(validAudience);
        validationParameters.RoleClaimType = ClaimTypes.Role;
        validationParameters.ClockSkew = TimeSpan.Zero;

        if (!tokenHandler.CanReadToken(token)) return null;
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal?.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        }
        catch (SecurityTokenException e)
        {
            return null;
        }
    }
}