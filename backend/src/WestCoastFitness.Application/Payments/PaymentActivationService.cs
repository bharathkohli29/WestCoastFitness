using WestCoastFitness.Domain.Entities;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Payments;

/// <summary>
/// Pure state-transition function for the "payment result -&gt; membership
/// activation" data-flow example. Mutates the in-memory entities it is
/// given and returns nothing; persistence is the caller's responsibility
/// (<see cref="Memberships.MembershipCommandService"/>), keeping this class
/// trivially unit-testable without a database.
/// </summary>
public class PaymentActivationService
{
    public void Apply(Payment payment, Membership membership, PaymentStatus outcome, DateTime processedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(payment);
        ArgumentNullException.ThrowIfNull(membership);

        payment.Status = outcome;
        payment.ProcessedAtUtc = processedAtUtc;

        switch (outcome)
        {
            case PaymentStatus.Succeeded:
                membership.Status = MembershipStatus.Active;
                break;
            case PaymentStatus.Failed:
                membership.Status = MembershipStatus.Pending;
                break;
            case PaymentStatus.Refunded:
                membership.Status = MembershipStatus.Cancelled;
                membership.CancelledAtUtc = processedAtUtc;
                membership.CancellationReason = "Payment refunded.";
                break;
            case PaymentStatus.Pending:
            default:
                // No membership state change while a payment is still pending.
                break;
        }
    }
}
