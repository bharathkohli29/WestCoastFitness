using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application.Abstractions;
using WestCoastFitness.Application.Scheduling;
using WestCoastFitness.Application.Scheduling.Dtos;
using WestCoastFitness.Domain.Entities;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/class-schedules")]
[Authorize]
public class ClassSchedulesController : ControllerBase
{
    private readonly ClassSchedulingCommandService _schedulingCommandService;
    private readonly IApplicationDbContext _db;
    private readonly IValidator<RegisterForClassRequest> _registerValidator;

    public ClassSchedulesController(
        ClassSchedulingCommandService schedulingCommandService,
        IApplicationDbContext db,
        IValidator<RegisterForClassRequest> registerValidator)
    {
        _schedulingCommandService = schedulingCommandService;
        _db = db;
        _registerValidator = registerValidator;
    }

    /// <summary>Lists upcoming, scheduled classes, optionally filtered by location.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetUpcoming([FromQuery] Guid? locationId, CancellationToken cancellationToken)
    {
        var query = _db.ClassSchedules
            .Include(s => s.FitnessClass)
            .Include(s => s.Trainer)
            .Where(s => s.Status == ScheduleStatus.Scheduled && s.StartsAtUtc >= DateTime.UtcNow);

        if (locationId is Guid id)
        {
            query = query.Where(s => s.FitnessClass!.ClubLocationId == id);
        }

        var schedules = await query
            .OrderBy(s => s.StartsAtUtc)
            .Select(s => new
            {
                s.Id,
                ClassName = s.FitnessClass!.Name,
                TrainerName = s.Trainer!.FirstName + " " + s.Trainer.LastName,
                s.StartsAtUtc,
                s.EndsAtUtc,
                s.Capacity,
                RegisteredCount = s.Registrations.Count(r => r.Status == RegistrationStatus.Registered),
            })
            .ToListAsync(cancellationToken);

        return Ok(schedules);
    }

    /// <summary>Registers a member for a class, or waitlists them if it is full.</summary>
    [HttpPost("register")]
    [Authorize(Roles = "Member,FrontDesk,Manager,Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterForClassRequest request, CancellationToken cancellationToken)
    {
        await _registerValidator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _schedulingCommandService.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    /// <summary>Cancels a member's class registration, promoting the next waitlisted member if a seat opens up.</summary>
    [HttpPost("cancel-registration")]
    [Authorize(Roles = "Member,FrontDesk,Manager,Admin")]
    public async Task<IActionResult> CancelRegistration([FromBody] CancelRegistrationRequest request, CancellationToken cancellationToken)
    {
        var result = await _schedulingCommandService.CancelAsync(request, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(new { message = result.Error });
    }
}
