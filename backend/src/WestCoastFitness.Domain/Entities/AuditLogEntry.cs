using WestCoastFitness.Domain.Common;

namespace WestCoastFitness.Domain.Entities;

/// <summary>
/// Immutable record of a state-changing action, written by
/// <c>IAuditTrailService</c> for the "audit/history information" requirement.
/// </summary>
public class AuditLogEntry : AuditableEntity
{
    public string EntityName { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public Guid? PerformedByUserId { get; set; }

    public string Details { get; set; } = string.Empty;
}
