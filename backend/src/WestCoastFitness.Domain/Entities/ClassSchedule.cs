using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class ClassSchedule : AuditableEntity
{
    public Guid FitnessClassId { get; set; }

    public FitnessClass? FitnessClass { get; set; }

    public Guid TrainerId { get; set; }

    public Trainer? Trainer { get; set; }

    public DateTime StartsAtUtc { get; set; }

    public DateTime EndsAtUtc { get; set; }

    public int Capacity { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;

    public ICollection<ClassRegistration> Registrations { get; set; } = new List<ClassRegistration>();

    public ICollection<WaitlistEntry> Waitlist { get; set; } = new List<WaitlistEntry>();
}
