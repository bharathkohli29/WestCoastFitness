using WestCoastFitness.Domain.Common;
using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Domain.Entities;

/// <summary>
/// Authentication identity for every actor in the system (member, trainer,
/// front-desk staff, manager or admin). Kept deliberately separate from
/// <see cref="Member"/> and <see cref="Trainer"/> so that staff accounts do
/// not need a membership record.
/// </summary>
public class ApplicationUser : AuditableEntity
{
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? MemberId { get; set; }

    public Member? Member { get; set; }

    public Guid? TrainerId { get; set; }

    public Trainer? Trainer { get; set; }
}
