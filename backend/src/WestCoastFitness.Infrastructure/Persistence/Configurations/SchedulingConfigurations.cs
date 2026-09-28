using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Infrastructure.Persistence.Configurations;

public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Bio).HasMaxLength(1000);
        builder.Property(x => x.Specialties).HasMaxLength(300);
        builder.Ignore(x => x.FullName);
        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasOne(x => x.HomeLocation)
            .WithMany(l => l.Trainers)
            .HasForeignKey(x => x.HomeLocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TrainerAvailabilityConfiguration : IEntityTypeConfiguration<TrainerAvailability>
{
    public void Configure(EntityTypeBuilder<TrainerAvailability> builder)
    {
        builder.HasOne(x => x.Trainer)
            .WithMany(t => t.Availability)
            .HasForeignKey(x => x.TrainerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FitnessClassConfiguration : IEntityTypeConfiguration<FitnessClass>
{
    public void Configure(EntityTypeBuilder<FitnessClass> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.HasOne(x => x.ClubLocation)
            .WithMany(l => l.FitnessClasses)
            .HasForeignKey(x => x.ClubLocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ClassScheduleConfiguration : IEntityTypeConfiguration<ClassSchedule>
{
    public void Configure(EntityTypeBuilder<ClassSchedule> builder)
    {
        builder.HasOne(x => x.FitnessClass)
            .WithMany(c => c.Schedules)
            .HasForeignKey(x => x.FitnessClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Trainer)
            .WithMany(t => t.ClassSchedules)
            .HasForeignKey(x => x.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.StartsAtUtc);
    }
}

public class ClassRegistrationConfiguration : IEntityTypeConfiguration<ClassRegistration>
{
    public void Configure(EntityTypeBuilder<ClassRegistration> builder)
    {
        builder.HasOne(x => x.ClassSchedule)
            .WithMany(s => s.Registrations)
            .HasForeignKey(x => x.ClassScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Member)
            .WithMany(m => m.ClassRegistrations)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ClassScheduleId, x.MemberId });
    }
}

public class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    public void Configure(EntityTypeBuilder<WaitlistEntry> builder)
    {
        builder.HasOne(x => x.ClassSchedule)
            .WithMany(s => s.Waitlist)
            .HasForeignKey(x => x.ClassScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Member)
            .WithMany(m => m.WaitlistEntries)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ClassScheduleId, x.Position });
    }
}

public class CheckInConfiguration : IEntityTypeConfiguration<CheckIn>
{
    public void Configure(EntityTypeBuilder<CheckIn> builder)
    {
        builder.HasOne(x => x.Member)
            .WithMany(m => m.CheckIns)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClubLocation)
            .WithMany()
            .HasForeignKey(x => x.ClubLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClassSchedule)
            .WithMany()
            .HasForeignKey(x => x.ClassScheduleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
