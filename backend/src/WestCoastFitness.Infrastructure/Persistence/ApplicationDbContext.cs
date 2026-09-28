using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application.Abstractions;
using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    public DbSet<ClubLocation> ClubLocations => Set<ClubLocation>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();

    public DbSet<Membership> Memberships => Set<Membership>();

    public DbSet<Trainer> Trainers => Set<Trainer>();

    public DbSet<TrainerAvailability> TrainerAvailabilities => Set<TrainerAvailability>();

    public DbSet<FitnessClass> FitnessClasses => Set<FitnessClass>();

    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();

    public DbSet<ClassRegistration> ClassRegistrations => Set<ClassRegistration>();

    public DbSet<WaitlistEntry> WaitlistEntries => Set<WaitlistEntry>();

    public DbSet<CheckIn> CheckIns => Set<CheckIn>();

    public DbSet<WorkoutPlan> WorkoutPlans => Set<WorkoutPlan>();

    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();

    public DbSet<FitnessGoal> FitnessGoals => Set<FitnessGoal>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<Promotion> Promotions => Set<Promotion>();

    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }
    }
}
