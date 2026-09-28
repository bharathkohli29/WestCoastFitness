using WestCoastFitness.Application.Abstractions;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Infrastructure.Services;

public class AuditTrailService : IAuditTrailService
{
    private readonly IApplicationDbContext _db;

    public AuditTrailService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task RecordAsync(string entityName, string entityId, string action, Guid? performedByUserId, string details, CancellationToken cancellationToken = default)
    {
        _db.AuditLogEntries.Add(new AuditLogEntry
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            PerformedByUserId = performedByUserId,
            Details = details,
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}
