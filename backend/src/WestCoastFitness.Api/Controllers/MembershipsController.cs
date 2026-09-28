using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WestCoastFitness.Application.Memberships;
using WestCoastFitness.Application.Memberships.Dtos;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/memberships")]
[Authorize]
public class MembershipsController : ControllerBase
{
    private readonly MembershipCommandService _membershipCommandService;
    private readonly IValidator<PurchaseMembershipRequest> _purchaseValidator;

    public MembershipsController(
        MembershipCommandService membershipCommandService,
        IValidator<PurchaseMembershipRequest> purchaseValidator)
    {
        _membershipCommandService = membershipCommandService;
        _purchaseValidator = purchaseValidator;
    }

    /// <summary>Purchases a new membership for a member, applying an optional promotion code.</summary>
    [HttpPost]
    [Authorize(Roles = "Member,FrontDesk,Manager,Admin")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseMembershipRequest request, CancellationToken cancellationToken)
    {
        await _purchaseValidator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _membershipCommandService.PurchaseAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    /// <summary>Renews an existing membership for another billing cycle.</summary>
    [HttpPost("renew")]
    [Authorize(Roles = "Member,FrontDesk,Manager,Admin")]
    public async Task<IActionResult> Renew([FromBody] RenewMembershipRequest request, CancellationToken cancellationToken)
    {
        var result = await _membershipCommandService.RenewAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    /// <summary>Cancels an active or pending membership.</summary>
    [HttpPost("cancel")]
    [Authorize(Roles = "Member,FrontDesk,Manager,Admin")]
    public async Task<IActionResult> Cancel([FromBody] CancelMembershipRequest request, CancellationToken cancellationToken)
    {
        var result = await _membershipCommandService.CancelAsync(request, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(new { message = result.Error });
    }
}
