using WestCoastFitness.Domain.Common;

namespace WestCoastFitness.Domain.Entities;

public class Trainer : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string Specialties { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Guid HomeLocationId { get; set; }

    public ClubLocation? HomeLocation { get; set; }

    public ICollection<TrainerAvailability> Availability { get; set; } = new List<TrainerAvailability>();

    public ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}

/// <summary>
/// A recurring weekly availability window for a trainer. Used by scheduling
/// logic for the "trainer availability -&gt; schedule -&gt; booking result"
/// data-flow example.
/// </summary>
public class TrainerAvailability : AuditableEntity
{
    public Guid TrainerId { get; set; }

    public Trainer? Trainer { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}
