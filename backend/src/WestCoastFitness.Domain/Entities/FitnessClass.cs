using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class FitnessClass : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ClassCategory Category { get; set; }

    public int DefaultCapacity { get; set; } = 20;

    public int DurationMinutes { get; set; } = 60;

    public Guid ClubLocationId { get; set; }

    public ClubLocation? ClubLocation { get; set; }

    public ICollection<ClassSchedule> Schedules { get; set; } = new List<ClassSchedule>();
}
