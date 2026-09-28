using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application.Abstractions;
using WestCoastFitness.Application.Common;
using WestCoastFitness.Application.Common.Exceptions;
using WestCoastFitness.Application.Memberships.Dtos;
using WestCoastFitness.Domain.Entities;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Memberships;

/// <summary>
/// Orchestrates membership purchase, renewal and cancellation. The pure
/// pricing calculation is delegated to <see cref="MembershipPricingService"/>
/// so persistence concerns stay separate from the price/discount/tax logic.
/// </summary>
public class MembershipCommandService
{
    private readonly IApplicationDbContext _db;
    private readonly MembershipPricingService _pricingService;
    private readonly IDateTimeProvider _clock;
    private readonly IAuditTrailService _auditTrail;

    public MembershipCommandService(
        IApplicationDbContext db,
        MembershipPricingService pricingService,
        IDateTimeProvider clock,
        IAuditTrailService auditTrail)
    {
        _db = db;
        _pricingService = pricingService;
        _clock = clock;
        _auditTrail = auditTrail;
    }

    public async Task<Result<MembershipDto>> PurchaseAsync(PurchaseMembershipRequest request, CancellationToken cancellationToken = default)
    {
        var member = await _db.Members.FindAsync([request.MemberId], cancellationToken)
            ?? throw new NotFoundException(nameof(Member), request.MemberId);

        var plan = await _db.MembershipPlans.FindAsync([request.MembershipPlanId], cancellationToken)
            ?? throw new NotFoundException(nameof(MembershipPlan), request.MembershipPlanId);

        if (!plan.IsActive)
        {
            return Result.Failure<MembershipDto>("The selected membership plan is no longer available for purchase.");
        }

        var promotion = await ResolvePromotionAsync(request.PromotionCode, cancellationToken);
        var today = _clock.TodayUtc;
        var pricing = _pricingService.Calculate(plan, plan.DurationMonths, promotion, today);

        var membership = new Membership
        {
            MemberId = member.Id,
            MembershipPlanId = plan.Id,
            StartDate = today,
            EndDate = today.AddMonths(plan.DurationMonths),
            Status = MembershipStatus.Pending,
            AutoRenew = true,
        };

        _db.Memberships.Add(membership);

        var invoice = new Invoice
        {
            MemberId = member.Id,
            MembershipId = membership.Id,
            PromotionId = promotion?.Id,
            IssueDate = today,
            DueDate = today.AddDays(7),
            Subtotal = pricing.Subtotal,
            DiscountAmount = pricing.DiscountAmount,
            TaxAmount = pricing.TaxAmount,
            TotalAmount = pricing.TotalAmount,
            Status = InvoiceStatus.Issued,
        };

        _db.Invoices.Add(invoice);

        if (promotion is not null)
        {
            promotion.TimesRedeemed += 1;
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _auditTrail.RecordAsync(nameof(Membership), membership.Id.ToString(), "Purchased", null,
            $"Plan={plan.Name}; Total={pricing.TotalAmount:C}", cancellationToken);

        return Result.Success(ToDto(membership, plan, pricing.TotalAmount));
    }

    public async Task<Result> CancelAsync(CancelMembershipRequest request, CancellationToken cancellationToken = default)
    {
        var membership = await _db.Memberships.FindAsync([request.MembershipId], cancellationToken)
            ?? throw new NotFoundException(nameof(Membership), request.MembershipId);

        if (membership.Status is MembershipStatus.Cancelled or MembershipStatus.Expired)
        {
            return Result.Failure("This membership is already cancelled or expired.");
        }

        membership.Status = MembershipStatus.Cancelled;
        membership.AutoRenew = false;
        membership.CancelledAtUtc = _clock.UtcNow;
        membership.CancellationReason = request.Reason;

        await _db.SaveChangesAsync(cancellationToken);
        await _auditTrail.RecordAsync(nameof(Membership), membership.Id.ToString(), "Cancelled", null, request.Reason, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<MembershipDto>> RenewAsync(RenewMembershipRequest request, CancellationToken cancellationToken = default)
    {
        var membership = await _db.Memberships
            .Include(m => m.MembershipPlan)
            .FirstOrDefaultAsync(m => m.Id == request.MembershipId, cancellationToken)
            ?? throw new NotFoundException(nameof(Membership), request.MembershipId);

        if (membership.MembershipPlan is null)
        {
            return Result.Failure<MembershipDto>("The membership plan for this membership could not be found.");
        }

        if (membership.Status == MembershipStatus.Cancelled)
        {
            return Result.Failure<MembershipDto>("A cancelled membership cannot be renewed; purchase a new one instead.");
        }

        var promotion = await ResolvePromotionAsync(request.PromotionCode, cancellationToken);
        var today = _clock.TodayUtc;
        var pricing = _pricingService.Calculate(membership.MembershipPlan, membership.MembershipPlan.DurationMonths, promotion, today);

        var renewalStart = membership.EndDate > today ? membership.EndDate : today;
        membership.EndDate = renewalStart.AddMonths(membership.MembershipPlan.DurationMonths);
        membership.Status = MembershipStatus.Active;

        await _db.SaveChangesAsync(cancellationToken);
        await _auditTrail.RecordAsync(nameof(Membership), membership.Id.ToString(), "Renewed", null,
            $"NewEndDate={membership.EndDate}; Total={pricing.TotalAmount:C}", cancellationToken);

        return Result.Success(ToDto(membership, membership.MembershipPlan, pricing.TotalAmount));
    }

    private async Task<Promotion?> ResolvePromotionAsync(string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return await _db.Promotions.FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    private static MembershipDto ToDto(Membership membership, MembershipPlan plan, decimal totalCharged) => new(
        membership.Id,
        membership.MemberId,
        membership.MembershipPlanId,
        plan.Name,
        membership.StartDate,
        membership.EndDate,
        membership.Status,
        membership.AutoRenew,
        totalCharged);
}
