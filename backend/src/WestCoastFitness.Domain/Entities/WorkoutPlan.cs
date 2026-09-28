using WestCoastFitness.Domain.Common;

namespace WestCoastFitness.Domain.Entities;

public class WorkoutPlan : AuditableEntity
{
    public Guid MemberId { get; set; }

    public Member? Member { get; set; }

    public Guid? TrainerId { get; set; }

    public Trainer? Trainer { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();
}

public class WorkoutExercise : AuditableEntity
{
    public Guid WorkoutPlanId { get; set; }

    public WorkoutPlan? WorkoutPlan { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Sets { get; set; }

    public int Reps { get; set; }

    public decimal? WeightKg { get; set; }

    public int? DurationSeconds { get; set; }

    public int OrderIndex { get; set; }

    public string? Notes { get; set; }
}
