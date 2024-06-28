using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtHelper
{
    private readonly IConfiguration _configuration;

    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username)
    {
        var jwtConfig = _configuration.GetSection("Jwt");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expirationMinutes = Convert.ToDouble(jwtConfig["ExpirationMinutes"]);
        var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, jwtConfig["Subject"]),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64),
            new Claim("username", username),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expiration).ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}