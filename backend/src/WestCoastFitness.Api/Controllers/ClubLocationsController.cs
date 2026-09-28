using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application.Abstractions;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/locations")]
public class ClubLocationsController : ControllerBase
{
    private readonly IApplicationDbContext _db;

    public ClubLocationsController(IApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>Lists every club location, used to power the location filter across the app.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var locations = await _db.ClubLocations
            .OrderBy(l => l.Name)
            .Select(l => new { l.Id, l.Name, l.City, l.State, l.PhoneNumber })
            .ToListAsync(cancellationToken);

        return Ok(locations);
    }
}
