using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class Membership : AuditableEntity
{
    public Guid MemberId { get; set; }

    public Member? Member { get; set; }

    public Guid MembershipPlanId { get; set; }

    public MembershipPlan? MembershipPlan { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public MembershipStatus Status { get; set; } = MembershipStatus.Pending;

    public bool AutoRenew { get; set; } = true;

    public DateTime? CancelledAtUtc { get; set; }

    public string? CancellationReason { get; set; }
}
