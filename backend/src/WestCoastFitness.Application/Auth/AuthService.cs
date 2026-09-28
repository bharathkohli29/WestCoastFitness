using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application.Abstractions;
using WestCoastFitness.Application.Auth.Dtos;
using WestCoastFitness.Application.Common;

namespace WestCoastFitness.Application.Auth;

/// <summary>
/// Authenticates a user by email/password. Deliberately returns the same
/// generic failure message whether the account does not exist or the
/// password is wrong, so responses never confirm which accounts exist
/// (safe error responses / no user enumeration).
/// </summary>
public class AuthService
{
    private const string InvalidCredentialsMessage = "The email or password is incorrect.";

    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IApplicationDbContext db, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return Result.Failure<LoginResponse>(InvalidCredentialsMessage);
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<LoginResponse>(InvalidCredentialsMessage);
        }

        var (token, expiresAtUtc) = _tokenGenerator.GenerateToken(user);
        return Result.Success(new LoginResponse(token, expiresAtUtc, user.Role, user.Id));
    }
}
