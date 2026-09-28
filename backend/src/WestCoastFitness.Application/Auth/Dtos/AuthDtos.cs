using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Auth.Dtos;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, UserRole Role, Guid UserId);
