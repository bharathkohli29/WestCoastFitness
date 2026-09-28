using WestCoastFitness.Domain.Entities;

namespace WestCoastFitness.Application.Scheduling;

public enum BookingResult
{
    Confirmed,
    RejectedOutsideAvailability,
    RejectedOverlappingSchedule,
}

/// <summary>
/// Pure booking-eligibility calculator for the "trainer availability -&gt;
/// schedule -&gt; booking result" data-flow example.
/// </summary>
public class TrainerAvailabilityService
{
    public BookingResult EvaluateBooking(
        IReadOnlyCollection<TrainerAvailability> availability,
        IReadOnlyCollection<(DateTime StartsAtUtc, DateTime EndsAtUtc)> existingSchedules,
        DateTime proposedStartUtc,
        DateTime proposedEndUtc)
    {
        ArgumentNullException.ThrowIfNull(availability);
        ArgumentNullException.ThrowIfNull(existingSchedules);

        if (proposedEndUtc <= proposedStartUtc)
        {
            throw new ArgumentException("The proposed end time must be after the start time.", nameof(proposedEndUtc));
        }

        if (!IsWithinAvailability(availability, proposedStartUtc, proposedEndUtc))
        {
            return BookingResult.RejectedOutsideAvailability;
        }

        if (HasOverlap(existingSchedules, proposedStartUtc, proposedEndUtc))
        {
            return BookingResult.RejectedOverlappingSchedule;
        }

        return BookingResult.Confirmed;
    }

    private static bool IsWithinAvailability(
        IReadOnlyCollection<TrainerAvailability> availability,
        DateTime proposedStartUtc,
        DateTime proposedEndUtc)
    {
        var dayOfWeek = proposedStartUtc.DayOfWeek;
        var startTime = TimeOnly.FromDateTime(proposedStartUtc);
        var endTime = TimeOnly.FromDateTime(proposedEndUtc);

        return availability.Any(window =>
            window.DayOfWeek == dayOfWeek &&
            window.StartTime <= startTime &&
            window.EndTime >= endTime);
    }

    private static bool HasOverlap(
        IReadOnlyCollection<(DateTime StartsAtUtc, DateTime EndsAtUtc)> existingSchedules,
        DateTime proposedStartUtc,
        DateTime proposedEndUtc)
    {
        return existingSchedules.Any(existing =>
            proposedStartUtc < existing.EndsAtUtc && proposedEndUtc > existing.StartsAtUtc);
    }
}
