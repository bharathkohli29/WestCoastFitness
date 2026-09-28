using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class Invoice : AuditableEntity
{
    public Guid MemberId { get; set; }

    public Member? Member { get; set; }

    public Guid? MembershipId { get; set; }

    public Membership? Membership { get; set; }

    public Guid? PromotionId { get; set; }

    public Promotion? Promotion { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly DueDate { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
