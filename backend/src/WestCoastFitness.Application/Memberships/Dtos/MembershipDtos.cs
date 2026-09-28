using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Memberships.Dtos;

public record PurchaseMembershipRequest(Guid MemberId, Guid MembershipPlanId, string? PromotionCode);

public record MembershipDto(
    Guid Id,
    Guid MemberId,
    Guid MembershipPlanId,
    string PlanName,
    DateOnly StartDate,
    DateOnly EndDate,
    MembershipStatus Status,
    bool AutoRenew,
    decimal TotalCharged);

public record CancelMembershipRequest(Guid MembershipId, string Reason);

public record RenewMembershipRequest(Guid MembershipId, string? PromotionCode);
