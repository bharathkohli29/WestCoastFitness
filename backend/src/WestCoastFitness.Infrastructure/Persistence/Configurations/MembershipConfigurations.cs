using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Infrastructure.Persistence.Configurations;

public class ClubLocationConfiguration : IEntityTypeConfiguration<ClubLocation>
{
    public void Configure(EntityTypeBuilder<ClubLocation> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.AddressLine1).IsRequired().HasMaxLength(200);
        builder.Property(x => x.City).IsRequired().HasMaxLength(100);
        builder.Property(x => x.State).IsRequired().HasMaxLength(2);
        builder.Property(x => x.ZipCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.HasIndex(x => x.Name);
    }
}

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Ignore(x => x.FullName);

        builder.HasOne(x => x.HomeLocation)
            .WithMany(l => l.Members)
            .HasForeignKey(x => x.HomeLocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class MembershipPlanConfiguration : IEntityTypeConfiguration<MembershipPlan>
{
    public void Configure(EntityTypeBuilder<MembershipPlan> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.MonthlyPrice).HasPrecision(10, 2);
    }
}

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.Property(x => x.CancellationReason).HasMaxLength(500);

        builder.HasOne(x => x.Member)
            .WithMany(m => m.Memberships)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MembershipPlan)
            .WithMany(p => p.Memberships)
            .HasForeignKey(x => x.MembershipPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.MemberId, x.Status });
    }
}
