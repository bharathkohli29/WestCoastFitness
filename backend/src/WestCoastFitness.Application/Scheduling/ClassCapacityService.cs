using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Scheduling;

public enum EnrollmentOutcome
{
    Registered,
    Waitlisted,
}

/// <summary>
/// Pure capacity calculator for the "class capacity -&gt; registrations -&gt;
/// waitlist availability" data-flow example. Deliberately has no database
/// access so it can be exhaustively boundary-tested: zero registrations,
/// capacity - 1, exactly at capacity, and over capacity.
/// </summary>
public class ClassCapacityService
{
    public int CountActiveRegistrations(IReadOnlyCollection<RegistrationStatus> statuses)
    {
        ArgumentNullException.ThrowIfNull(statuses);

        return statuses.Count(s => s is RegistrationStatus.Registered or RegistrationStatus.Attended);
    }

    public EnrollmentOutcome DetermineOutcome(int capacity, int activeRegistrationCount)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Class capacity must be greater than zero.");
        }

        return activeRegistrationCount < capacity
            ? EnrollmentOutcome.Registered
            : EnrollmentOutcome.Waitlisted;
    }

    public int NextWaitlistPosition(IReadOnlyCollection<int> existingPositions)
    {
        ArgumentNullException.ThrowIfNull(existingPositions);

        return existingPositions.Count == 0 ? 1 : existingPositions.Max() + 1;
    }

    /// <summary>
    /// Given a registration cancellation, decides which waitlisted member
    /// (if any) should be promoted into the freed seat: the entry with the
    /// lowest position, i.e. the one who has been waiting longest.
    /// </summary>
    public Guid? SelectPromotionCandidate(IReadOnlyDictionary<Guid, int> waitlistEntryPositions)
    {
        ArgumentNullException.ThrowIfNull(waitlistEntryPositions);

        if (waitlistEntryPositions.Count == 0)
        {
            return null;
        }

        return waitlistEntryPositions.OrderBy(kvp => kvp.Value).First().Key;
    }
}
