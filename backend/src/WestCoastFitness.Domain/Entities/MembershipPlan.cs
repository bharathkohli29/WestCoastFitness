using WestCoastFitness.Domain.Common;

namespace WestCoastFitness.Domain.Entities;

public class MembershipPlan : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal MonthlyPrice { get; set; }

    public int DurationMonths { get; set; } = 1;

    /// <summary>Null means unlimited classes per month.</summary>
    public int? MaxClassesPerMonth { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
