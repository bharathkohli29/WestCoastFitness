using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Trainer)
            .WithMany()
            .HasForeignKey(x => x.TrainerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.Property(x => x.EntityName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.EntityId).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Details).HasMaxLength(2000);
        builder.HasIndex(x => new { x.EntityName, x.EntityId });
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(1000);
        builder.HasIndex(x => new { x.UserId, x.ReadAtUtc });
    }
}
