using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class JwtHelper
{
    private static readonly string SecretKey = ConfigurationManager.AppSettings["Jwt:SecretKey"];
    private static readonly string Issuer = ConfigurationManager.AppSettings["Jwt:Issuer"];
    private static readonly string Audience = ConfigurationManager.AppSettings["Jwt:Audience"];
    private static readonly int ExpiresInMinutes = int.TryParse(ConfigurationManager.AppSettings["Jwt:ExpiresInMinutes"], out var mins) ? mins : 60;


    public static string GenerateToken(string userId, string phoneNumber)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.MobilePhone, phoneNumber)
    };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(ExpiresInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
