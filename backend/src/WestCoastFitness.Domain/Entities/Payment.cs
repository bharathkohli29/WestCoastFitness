using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class Payment : AuditableEntity
{
    public Guid MemberId { get; set; }

    public Member? Member { get; set; }

    public Guid? InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "USD";

    public PaymentMethod Method { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTime? ProcessedAtUtc { get; set; }

    public string TransactionReference { get; set; } = string.Empty;
}
