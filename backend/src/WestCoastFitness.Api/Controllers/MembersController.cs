using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application.Abstractions;
using WestCoastFitness.Application.Access;
using WestCoastFitness.Application.Common.Exceptions;
using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/members")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    private readonly MemberAccessService _accessService;

    public MembersController(IApplicationDbContext db, MemberAccessService accessService)
    {
        _db = db;
        _accessService = accessService;
    }

    /// <summary>Returns a member's profile summary.</summary>
    [HttpGet("{memberId:guid}")]
    [Authorize(Roles = "Member,Trainer,FrontDesk,Manager,Admin")]
    public async Task<IActionResult> GetById(Guid memberId, CancellationToken cancellationToken)
    {
        var member = await _db.Members
            .Include(m => m.HomeLocation)
            .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken)
            ?? throw new NotFoundException(nameof(Member), memberId);

        return Ok(new
        {
            member.Id,
            member.FullName,
            member.Email,
            member.Status,
            HomeLocation = member.HomeLocation?.Name,
            member.JoinDate,
        });
    }

    /// <summary>
    /// Evaluates whether a member currently has gym-floor / class access,
    /// based on account status and active membership expiration.
    /// </summary>
    [HttpGet("{memberId:guid}/access")]
    [Authorize(Roles = "FrontDesk,Manager,Admin")]
    public async Task<IActionResult> EvaluateAccess(Guid memberId, CancellationToken cancellationToken)
    {
        var member = await _db.Members.FindAsync([memberId], cancellationToken)
            ?? throw new NotFoundException(nameof(Member), memberId);

        var memberships = await _db.Memberships
            .Where(m => m.MemberId == memberId)
            .ToListAsync(cancellationToken);

        var decision = _accessService.EvaluateAccess(member, memberships, DateOnly.FromDateTime(DateTime.UtcNow));

        return Ok(new { memberId, decision = decision.ToString() });
    }
}
