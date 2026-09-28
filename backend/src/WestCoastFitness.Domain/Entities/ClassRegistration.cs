using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class ClassRegistration : AuditableEntity
{
    public Guid ClassScheduleId { get; set; }

    public ClassSchedule? ClassSchedule { get; set; }

    public Guid MemberId { get; set; }

    public Member? Member { get; set; }

    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Registered;
}
