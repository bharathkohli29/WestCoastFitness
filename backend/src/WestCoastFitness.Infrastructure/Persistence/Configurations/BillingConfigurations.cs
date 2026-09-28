using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(x => x.Subtotal).HasPrecision(10, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(10, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(10, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(10, 2);

        builder.HasOne(x => x.Member)
            .WithMany(m => m.Invoices)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Membership)
            .WithMany()
            .HasForeignKey(x => x.MembershipId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Promotion)
            .WithMany()
            .HasForeignKey(x => x.PromotionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(10, 2);
        builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);
        builder.Property(x => x.TransactionReference).HasMaxLength(100);

        builder.HasOne(x => x.Member)
            .WithMany(m => m.Payments)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Invoice)
            .WithMany(i => i.Payments)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.Property(x => x.Code).IsRequired().HasMaxLength(32);
        builder.Property(x => x.Description).HasMaxLength(300);
        builder.Property(x => x.DiscountValue).HasPrecision(10, 2);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
