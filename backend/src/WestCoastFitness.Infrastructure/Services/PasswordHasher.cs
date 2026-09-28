using WestCoastFitness.Application.Abstractions;

namespace WestCoastFitness.Infrastructure.Services;

/// <summary>
/// BCrypt-based password hashing. Work factor 12 balances brute-force
/// resistance against request latency; never store or log plaintext
/// passwords anywhere in this codebase (secure password handling).
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainTextPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainTextPassword, WorkFactor);

    public bool Verify(string plainTextPassword, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(plainTextPassword, passwordHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // A malformed stored hash must never crash the login flow;
            // treat it as a verification failure.
            return false;
        }
    }
}
