using WestCoastFitness.Domain.Common;

namespace WestCoastFitness.Domain.Entities;

public class ClubLocation : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string AddressLine1 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = "AZ";

    public string ZipCode { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string TimeZoneId { get; set; } = "America/Phoenix";

    public ICollection<Member> Members { get; set; } = new List<Member>();

    public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();

    public ICollection<FitnessClass> FitnessClasses { get; set; } = new List<FitnessClass>();
}
