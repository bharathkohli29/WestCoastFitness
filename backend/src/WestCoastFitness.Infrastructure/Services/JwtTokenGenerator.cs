using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WestCoastFitness.Application.Auth;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Infrastructure.Services;

public class JwtOptions
{
    public string SigningKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = "WestCoastFitness";

    public string Audience { get; set; } = "WestCoastFitness.Client";

    public int ExpiryMinutes { get; set; } = 60;
}

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _options = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

        if (string.IsNullOrWhiteSpace(_options.SigningKey))
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey is not configured. Set it via configuration or the JWT__SIGNINGKEY environment variable.");
        }
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
