using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class FitnessGoal : AuditableEntity
{
    public Guid MemberId { get; set; }

    public Member? Member { get; set; }

    public GoalType GoalType { get; set; }

    public decimal TargetValue { get; set; }

    public decimal CurrentValue { get; set; }

    public string Unit { get; set; } = string.Empty;

    public DateOnly TargetDate { get; set; }

    public GoalStatus Status { get; set; } = GoalStatus.InProgress;
}
