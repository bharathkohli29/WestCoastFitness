using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }

    DateOnly TodayUtc { get; }
}

public interface IPasswordHasher
{
    string Hash(string plainTextPassword);

    bool Verify(string plainTextPassword, string passwordHash);
}

/// <summary>
/// Writes immutable audit records for state-changing actions. Kept as a
/// narrow, single-purpose abstraction rather than folded into every
/// service, per the Clean Architecture / SRP guidance in the benchmark
/// spec.
/// </summary>
public interface IAuditTrailService
{
    Task RecordAsync(string entityName, string entityId, string action, Guid? performedByUserId, string details, CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }

    Domain.Enums.UserRole? Role { get; }
}

public interface INotificationService
{
    Task NotifyAsync(Guid userId, Domain.Enums.NotificationType type, string title, string message, CancellationToken cancellationToken = default);
}
