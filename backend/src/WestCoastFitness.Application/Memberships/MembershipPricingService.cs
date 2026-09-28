using WestCoastFitness.Domain.Entities;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Memberships;

public record PricingBreakdown(
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal TotalAmount);

/// <summary>
/// Pure, side-effect-free pricing calculator: this is the canonical
/// "membership price -&gt; discount -&gt; tax -&gt; final charge" data-flow example
/// called out in the benchmark spec. Every branch here is intentionally
/// small and independently testable (definition-use pairs: <c>subtotal</c>,
/// <c>discountAmount</c> and <c>taxAmount</c> are each defined once and used
/// exactly once downstream).
/// </summary>
public class MembershipPricingService
{
    private const decimal ArizonaSalesTaxRate = 0.086m;

    public PricingBreakdown Calculate(MembershipPlan plan, int months, Promotion? promotion, DateOnly asOf)
    {
        ArgumentNullException.ThrowIfNull(plan);

        if (months <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(months), months, "Months must be a positive number.");
        }

        var subtotal = plan.MonthlyPrice * months;
        var discountAmount = CalculateDiscount(subtotal, promotion, asOf);
        var discountedSubtotal = subtotal - discountAmount;
        var taxAmount = Math.Round(discountedSubtotal * ArizonaSalesTaxRate, 2, MidpointRounding.AwayFromZero);
        var totalAmount = discountedSubtotal + taxAmount;

        return new PricingBreakdown(subtotal, discountAmount, taxAmount, totalAmount);
    }

    private static decimal CalculateDiscount(decimal subtotal, Promotion? promotion, DateOnly asOf)
    {
        if (promotion is null || !promotion.IsActive)
        {
            return 0m;
        }

        if (asOf < promotion.StartDate || asOf > promotion.EndDate)
        {
            return 0m;
        }

        if (promotion.MaxRedemptions is int max && promotion.TimesRedeemed >= max)
        {
            return 0m;
        }

        var rawDiscount = promotion.DiscountType == DiscountType.Percentage
            ? subtotal * (promotion.DiscountValue / 100m)
            : promotion.DiscountValue;

        // A discount can never exceed the subtotal it is applied to.
        return Math.Min(rawDiscount, subtotal);
    }
}
