using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class Member : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public DateOnly JoinDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public MemberStatus Status { get; set; } = MemberStatus.PendingActivation;

    public Guid HomeLocationId { get; set; }

    public ClubLocation? HomeLocation { get; set; }

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public ICollection<ClassRegistration> ClassRegistrations { get; set; } = new List<ClassRegistration>();

    public ICollection<WaitlistEntry> WaitlistEntries { get; set; } = new List<WaitlistEntry>();

    public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();

    public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();

    public ICollection<FitnessGoal> FitnessGoals { get; set; } = new List<FitnessGoal>();

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}
