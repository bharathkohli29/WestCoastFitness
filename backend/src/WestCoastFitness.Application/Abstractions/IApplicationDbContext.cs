using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Application.Abstractions;

/// <summary>
/// Persistence seam the Application layer depends on instead of the
/// concrete EF Core DbContext, so services can be unit-tested against an
/// in-memory provider without referencing Infrastructure (dependency
/// inversion between layers).
/// </summary>
public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }

    DbSet<ClubLocation> ClubLocations { get; }

    DbSet<Member> Members { get; }

    DbSet<MembershipPlan> MembershipPlans { get; }

    DbSet<Membership> Memberships { get; }

    DbSet<Trainer> Trainers { get; }

    DbSet<TrainerAvailability> TrainerAvailabilities { get; }

    DbSet<FitnessClass> FitnessClasses { get; }

    DbSet<ClassSchedule> ClassSchedules { get; }

    DbSet<ClassRegistration> ClassRegistrations { get; }

    DbSet<WaitlistEntry> WaitlistEntries { get; }

    DbSet<CheckIn> CheckIns { get; }

    DbSet<WorkoutPlan> WorkoutPlans { get; }

    DbSet<WorkoutExercise> WorkoutExercises { get; }

    DbSet<FitnessGoal> FitnessGoals { get; }

    DbSet<Invoice> Invoices { get; }

    DbSet<Payment> Payments { get; }

    DbSet<Promotion> Promotions { get; }

    DbSet<AuditLogEntry> AuditLogEntries { get; }

    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
