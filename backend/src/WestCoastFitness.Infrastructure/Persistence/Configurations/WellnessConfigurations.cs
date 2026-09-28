using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Infrastructure.Persistence.Configurations;

public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
{
    public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.HasOne(x => x.Member)
            .WithMany(m => m.WorkoutPlans)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Trainer)
            .WithMany()
            .HasForeignKey(x => x.TrainerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.WeightKg).HasPrecision(6, 2);
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.HasOne(x => x.WorkoutPlan)
            .WithMany(p => p.Exercises)
            .HasForeignKey(x => x.WorkoutPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FitnessGoalConfiguration : IEntityTypeConfiguration<FitnessGoal>
{
    public void Configure(EntityTypeBuilder<FitnessGoal> builder)
    {
        builder.Property(x => x.TargetValue).HasPrecision(8, 2);
        builder.Property(x => x.CurrentValue).HasPrecision(8, 2);
        builder.Property(x => x.Unit).HasMaxLength(20);

        builder.HasOne(x => x.Member)
            .WithMany(m => m.FitnessGoals)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
