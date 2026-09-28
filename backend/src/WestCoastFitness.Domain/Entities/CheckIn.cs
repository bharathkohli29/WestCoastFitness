using WestCoastFitness.Domain.Common;

namespace WestCoastFitness.Domain.Entities;

public class CheckIn : AuditableEntity
{
    public Guid MemberId { get; set; }

    public Member? Member { get; set; }

    public Guid ClubLocationId { get; set; }

    public ClubLocation? ClubLocation { get; set; }

    public Guid? ClassScheduleId { get; set; }

    public ClassSchedule? ClassSchedule { get; set; }

    public DateTime CheckedInAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CheckedOutAtUtc { get; set; }
}
