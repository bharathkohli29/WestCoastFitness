using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

public class Promotion : AuditableEntity
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DiscountType DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? MaxRedemptions { get; set; }

    public int TimesRedeemed { get; set; }

    public bool IsActive { get; set; } = true;
}
