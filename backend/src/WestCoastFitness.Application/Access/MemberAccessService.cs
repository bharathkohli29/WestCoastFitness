using WestCoastFitness.Domain.Entities;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Access;

public enum AccessDecision
{
    Granted,
    DeniedMemberSuspended,
    DeniedMemberCancelled,
    DeniedNoActiveMembership,
    DeniedMembershipExpired,
}

/// <summary>
/// Pure decision function for the "member status -&gt; membership expiration
/// -&gt; access decision" data-flow example. Front-desk check-in and class
/// registration both call through this single source of truth rather than
/// duplicating the eligibility rules.
/// </summary>
public class MemberAccessService
{
    public AccessDecision EvaluateAccess(Member member, IReadOnlyCollection<Membership> memberships, DateOnly asOf)
    {
        ArgumentNullException.ThrowIfNull(member);
        ArgumentNullException.ThrowIfNull(memberships);

        if (member.Status == MemberStatus.Suspended)
        {
            return AccessDecision.DeniedMemberSuspended;
        }

        if (member.Status == MemberStatus.Cancelled)
        {
            return AccessDecision.DeniedMemberCancelled;
        }

        var activeMembership = memberships
            .Where(m => m.Status == MembershipStatus.Active)
            .OrderByDescending(m => m.EndDate)
            .FirstOrDefault();

        if (activeMembership is null)
        {
            return AccessDecision.DeniedNoActiveMembership;
        }

        return asOf > activeMembership.EndDate
            ? AccessDecision.DeniedMembershipExpired
            : AccessDecision.Granted;
    }
}
