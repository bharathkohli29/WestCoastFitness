namespace WestCoastFitness.Domain.Common;

/// <summary>
/// Base type for every persisted aggregate/entity in the system.
/// Carries a stable identity plus the audit timestamps used by
/// history/audit-trail features and by data-flow examples such as
/// "member status -&gt; membership expiration -&gt; access decision".
/// </summary>
public abstract class AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}
