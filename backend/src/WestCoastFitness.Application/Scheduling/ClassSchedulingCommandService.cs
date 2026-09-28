using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application.Abstractions;
using WestCoastFitness.Application.Common;
using WestCoastFitness.Application.Common.Exceptions;
using WestCoastFitness.Application.Scheduling.Dtos;
using WestCoastFitness.Domain.Entities;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Scheduling;

/// <summary>
/// Orchestrates class registration, cancellation and waitlist promotion.
/// Delegates the capacity/waitlist arithmetic to the pure
/// <see cref="ClassCapacityService"/> so that logic can be exhaustively
/// boundary-tested independent of the database.
/// </summary>
public class ClassSchedulingCommandService
{
    private readonly IApplicationDbContext _db;
    private readonly ClassCapacityService _capacityService;
    private readonly IDateTimeProvider _clock;
    private readonly IAuditTrailService _auditTrail;

    public ClassSchedulingCommandService(
        IApplicationDbContext db,
        ClassCapacityService capacityService,
        IDateTimeProvider clock,
        IAuditTrailService auditTrail)
    {
        _db = db;
        _capacityService = capacityService;
        _clock = clock;
        _auditTrail = auditTrail;
    }

    public async Task<Result<ClassRegistrationResultDto>> RegisterAsync(RegisterForClassRequest request, CancellationToken cancellationToken = default)
    {
        var schedule = await _db.ClassSchedules
            .Include(s => s.Registrations)
            .Include(s => s.Waitlist)
            .FirstOrDefaultAsync(s => s.Id == request.ClassScheduleId, cancellationToken)
            ?? throw new NotFoundException(nameof(ClassSchedule), request.ClassScheduleId);

        if (schedule.Status != ScheduleStatus.Scheduled)
        {
            return Result.Failure<ClassRegistrationResultDto>("This class is no longer accepting registrations.");
        }

        var alreadyRegistered = schedule.Registrations.Any(r =>
            r.MemberId == request.MemberId &&
            r.Status is RegistrationStatus.Registered or RegistrationStatus.Waitlisted);

        if (alreadyRegistered)
        {
            return Result.Failure<ClassRegistrationResultDto>("This member is already registered for this class.");
        }

        var activeCount = _capacityService.CountActiveRegistrations(
            schedule.Registrations.Select(r => r.Status).ToList());
        var outcome = _capacityService.DetermineOutcome(schedule.Capacity, activeCount);

        var registration = new ClassRegistration
        {
            ClassScheduleId = schedule.Id,
            MemberId = request.MemberId,
            RegisteredAtUtc = _clock.UtcNow,
            Status = outcome == EnrollmentOutcome.Registered
                ? RegistrationStatus.Registered
                : RegistrationStatus.Waitlisted,
        };

        _db.ClassRegistrations.Add(registration);

        int? waitlistPosition = null;

        if (outcome == EnrollmentOutcome.Waitlisted)
        {
            var nextPosition = _capacityService.NextWaitlistPosition(
                schedule.Waitlist.Select(w => w.Position).ToList());

            _db.WaitlistEntries.Add(new WaitlistEntry
            {
                ClassScheduleId = schedule.Id,
                MemberId = request.MemberId,
                JoinedAtUtc = _clock.UtcNow,
                Position = nextPosition,
            });

            waitlistPosition = nextPosition;
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _auditTrail.RecordAsync(nameof(ClassRegistration), registration.Id.ToString(), "Created", null,
            $"Outcome={outcome}", cancellationToken);

        return Result.Success(new ClassRegistrationResultDto(
            registration.Id, schedule.Id, request.MemberId, registration.Status, waitlistPosition));
    }

    public async Task<Result> CancelAsync(CancelRegistrationRequest request, CancellationToken cancellationToken = default)
    {
        var registration = await _db.ClassRegistrations.FindAsync([request.ClassRegistrationId], cancellationToken)
            ?? throw new NotFoundException(nameof(ClassRegistration), request.ClassRegistrationId);

        if (registration.Status is RegistrationStatus.CancelledByMember or RegistrationStatus.CancelledByClub)
        {
            return Result.Failure("This registration has already been cancelled.");
        }

        var wasActiveSeat = registration.Status == RegistrationStatus.Registered;
        registration.Status = RegistrationStatus.CancelledByMember;

        if (wasActiveSeat)
        {
            await PromoteFromWaitlistAsync(registration.ClassScheduleId, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _auditTrail.RecordAsync(nameof(ClassRegistration), registration.Id.ToString(), "Cancelled", null, string.Empty, cancellationToken);

        return Result.Success();
    }

    private async Task PromoteFromWaitlistAsync(Guid classScheduleId, CancellationToken cancellationToken)
    {
        var waitlist = await _db.WaitlistEntries
            .Where(w => w.ClassScheduleId == classScheduleId && w.PromotedAtUtc == null)
            .ToListAsync(cancellationToken);

        var candidateId = _capacityService.SelectPromotionCandidate(
            waitlist.ToDictionary(w => w.Id, w => w.Position));

        if (candidateId is null)
        {
            return;
        }

        var candidate = waitlist.Single(w => w.Id == candidateId);
        candidate.PromotedAtUtc = _clock.UtcNow;

        var promotedRegistration = await _db.ClassRegistrations
            .FirstOrDefaultAsync(r => r.ClassScheduleId == classScheduleId && r.MemberId == candidate.MemberId
                && r.Status == RegistrationStatus.Waitlisted, cancellationToken);

        if (promotedRegistration is not null)
        {
            promotedRegistration.Status = RegistrationStatus.Registered;
        }
    }
}
