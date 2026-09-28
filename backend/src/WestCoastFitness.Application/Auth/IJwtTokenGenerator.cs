using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Application.Auth;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user);
}
