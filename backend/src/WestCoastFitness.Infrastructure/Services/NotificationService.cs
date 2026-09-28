using WestCoastFitness.Application.Abstractions;
using WestCoastFitness.Domain.Entities;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Infrastructure.Services;

/// <summary>
/// In-app notification writer. A real deployment would also fan these out
/// to email/SMS via a message queue; that integration is intentionally out
/// of scope for this benchmark (no outbound network calls from tests).
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _db;

    public NotificationService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task NotifyAsync(Guid userId, NotificationType type, string title, string message, CancellationToken cancellationToken = default)
    {
        _db.Notifications.Add(new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}
