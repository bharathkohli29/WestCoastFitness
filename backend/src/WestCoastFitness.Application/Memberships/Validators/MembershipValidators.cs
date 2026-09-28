using FluentValidation;
using WestCoastFitness.Application.Memberships.Dtos;

namespace WestCoastFitness.Application.Memberships.Validators;

public class PurchaseMembershipRequestValidator : AbstractValidator<PurchaseMembershipRequest>
{
    public PurchaseMembershipRequestValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.MembershipPlanId).NotEmpty();
        RuleFor(x => x.PromotionCode)
            .MaximumLength(32)
            .When(x => !string.IsNullOrWhiteSpace(x.PromotionCode));
    }
}

public class CancelMembershipRequestValidator : AbstractValidator<CancelMembershipRequest>
{
    public CancelMembershipRequestValidator()
    {
        RuleFor(x => x.MembershipId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class RenewMembershipRequestValidator : AbstractValidator<RenewMembershipRequest>
{
    public RenewMembershipRequestValidator()
    {
        RuleFor(x => x.MembershipId).NotEmpty();
        RuleFor(x => x.PromotionCode)
            .MaximumLength(32)
            .When(x => !string.IsNullOrWhiteSpace(x.PromotionCode));
    }
}
