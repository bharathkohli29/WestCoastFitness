using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application.Abstractions;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/membership-plans")]
public class MembershipPlansController : ControllerBase
{
    private readonly IApplicationDbContext _db;

    public MembershipPlansController(IApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>Lists the currently purchasable membership plans.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActivePlans(CancellationToken cancellationToken)
    {
        var plans = await _db.MembershipPlans
            .Where(p => p.IsActive)
            .OrderBy(p => p.MonthlyPrice)
            .ToListAsync(cancellationToken);

        return Ok(plans);
    }
}
