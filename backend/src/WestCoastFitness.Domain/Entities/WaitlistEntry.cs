using WestCoastFitness.Domain.Common;

namespace WestCoastFitness.Domain.Entities;

public class WaitlistEntry : AuditableEntity
{
    public Guid ClassScheduleId { get; set; }

    public ClassSchedule? ClassSchedule { get; set; }

    public Guid MemberId { get; set; }

    public Member? Member { get; set; }

    public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>1-based queue position at the moment the entry was created.</summary>
    public int Position { get; set; }

    public DateTime? PromotedAtUtc { get; set; }
}
