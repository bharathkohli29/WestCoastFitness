using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Domain.Entities;
using WestCoastFitness.Domain.Enums;
using WestCoastFitness.Infrastructure.Services;

namespace WestCoastFitness.Infrastructure.Persistence.Seed;

/// <summary>
/// Deterministic development/demo seed data: three Arizona locations,
/// four membership plans, a roster of trainers and members, and a week of
/// scheduled classes. All identifiers are fixed GUIDs so the seed is
/// reproducible across environments and CI runs.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.ClubLocations.AnyAsync(cancellationToken))
        {
            return;
        }

        var hasher = new PasswordHasher();

        var locations = SeedLocations();
        db.ClubLocations.AddRange(locations);

        var plans = SeedMembershipPlans();
        db.MembershipPlans.AddRange(plans);

        var promotions = SeedPromotions();
        db.Promotions.AddRange(promotions);

        var trainers = SeedTrainers(locations);
        db.Trainers.AddRange(trainers);

        foreach (var trainer in trainers)
        {
            db.TrainerAvailabilities.AddRange(SeedAvailability(trainer));
        }

        var fitnessClasses = SeedFitnessClasses(locations);
        db.FitnessClasses.AddRange(fitnessClasses);

        var schedules = SeedSchedules(fitnessClasses, trainers);
        db.ClassSchedules.AddRange(schedules);

        var members = SeedMembers(locations);
        db.Members.AddRange(members);

        var memberships = SeedMemberships(members, plans);
        db.Memberships.AddRange(memberships);

        db.Users.AddRange(SeedUsers(hasher, members, trainers));

        await db.SaveChangesAsync(cancellationToken);
    }

    private static List<ClubLocation> SeedLocations() =>
    [
        new()
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
            Name = "West Coast Fitness - Scottsdale",
            AddressLine1 = "7301 E Indian Bend Rd",
            City = "Scottsdale",
            State = "AZ",
            ZipCode = "85250",
            PhoneNumber = "480-555-0101",
        },
        new()
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000002"),
            Name = "West Coast Fitness - Tempe",
            AddressLine1 = "1825 E Rio Salado Pkwy",
            City = "Tempe",
            State = "AZ",
            ZipCode = "85281",
            PhoneNumber = "480-555-0102",
        },
        new()
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000003"),
            Name = "West Coast Fitness - Chandler",
            AddressLine1 = "3111 W Chandler Blvd",
            City = "Chandler",
            State = "AZ",
            ZipCode = "85226",
            PhoneNumber = "480-555-0103",
        },
    ];

    private static List<MembershipPlan> SeedMembershipPlans() =>
    [
        new()
        {
            Id = Guid.Parse("22222222-0000-0000-0000-000000000001"),
            Name = "Basic Monthly",
            Description = "Single-location gym floor access.",
            MonthlyPrice = 39.99m,
            DurationMonths = 1,
            MaxClassesPerMonth = 4,
        },
        new()
        {
            Id = Guid.Parse("22222222-0000-0000-0000-000000000002"),
            Name = "Premium Monthly",
            Description = "All-location access plus unlimited classes.",
            MonthlyPrice = 69.99m,
            DurationMonths = 1,
            MaxClassesPerMonth = null,
        },
        new()
        {
            Id = Guid.Parse("22222222-0000-0000-0000-000000000003"),
            Name = "Annual",
            Description = "All-location access, unlimited classes, billed yearly.",
            MonthlyPrice = 59.99m,
            DurationMonths = 12,
            MaxClassesPerMonth = null,
        },
        new()
        {
            Id = Guid.Parse("22222222-0000-0000-0000-000000000004"),
            Name = "Student Monthly",
            Description = "Discounted single-location access for students.",
            MonthlyPrice = 24.99m,
            DurationMonths = 1,
            MaxClassesPerMonth = 4,
        },
    ];

    private static List<Promotion> SeedPromotions() =>
    [
        new()
        {
            Id = Guid.Parse("33333333-0000-0000-0000-000000000001"),
            Code = "WELCOME10",
            Description = "10% off a member's first purchase.",
            DiscountType = DiscountType.Percentage,
            DiscountValue = 10m,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31),
            MaxRedemptions = 500,
        },
        new()
        {
            Id = Guid.Parse("33333333-0000-0000-0000-000000000002"),
            Code = "SUMMER25",
            Description = "$25 off any annual plan.",
            DiscountType = DiscountType.FixedAmount,
            DiscountValue = 25m,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 8, 31),
            MaxRedemptions = 200,
        },
    ];

    private static List<Trainer> SeedTrainers(List<ClubLocation> locations) =>
    [
        new()
        {
            Id = Guid.Parse("44444444-0000-0000-0000-000000000001"),
            FirstName = "Maria",
            LastName = "Lopez",
            Email = "maria.lopez@westcoastfitness.example",
            Bio = "Certified strength & conditioning coach with 8 years of experience.",
            Specialties = "Strength, HIIT",
            HomeLocationId = locations[0].Id,
        },
        new()
        {
            Id = Guid.Parse("44444444-0000-0000-0000-000000000002"),
            FirstName = "David",
            LastName = "Chen",
            Email = "david.chen@westcoastfitness.example",
            Bio = "Yoga and mobility specialist, RYT-500 certified.",
            Specialties = "Yoga, Recovery",
            HomeLocationId = locations[1].Id,
        },
        new()
        {
            Id = Guid.Parse("44444444-0000-0000-0000-000000000003"),
            FirstName = "Ava",
            LastName = "Thompson",
            Email = "ava.thompson@westcoastfitness.example",
            Bio = "Indoor cycling and cardio-endurance coach.",
            Specialties = "Cycling, Cardio",
            HomeLocationId = locations[2].Id,
        },
    ];

    private static List<TrainerAvailability> SeedAvailability(Trainer trainer) =>
    [
        new()
        {
            TrainerId = trainer.Id,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(6, 0),
            EndTime = new TimeOnly(14, 0),
        },
        new()
        {
            TrainerId = trainer.Id,
            DayOfWeek = DayOfWeek.Wednesday,
            StartTime = new TimeOnly(6, 0),
            EndTime = new TimeOnly(14, 0),
        },
        new()
        {
            TrainerId = trainer.Id,
            DayOfWeek = DayOfWeek.Friday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
        },
    ];

    private static List<FitnessClass> SeedFitnessClasses(List<ClubLocation> locations) =>
    [
        new()
        {
            Id = Guid.Parse("55555555-0000-0000-0000-000000000001"),
            Name = "Morning HIIT",
            Description = "High-intensity interval training to start the day.",
            Category = ClassCategory.Hiit,
            DefaultCapacity = 16,
            DurationMinutes = 45,
            ClubLocationId = locations[0].Id,
        },
        new()
        {
            Id = Guid.Parse("55555555-0000-0000-0000-000000000002"),
            Name = "Vinyasa Flow",
            Description = "All-levels vinyasa yoga session.",
            Category = ClassCategory.Yoga,
            DefaultCapacity = 20,
            DurationMinutes = 60,
            ClubLocationId = locations[1].Id,
        },
        new()
        {
            Id = Guid.Parse("55555555-0000-0000-0000-000000000003"),
            Name = "Power Cycle",
            Description = "Indoor cycling with interval-based power targets.",
            Category = ClassCategory.Cycling,
            DefaultCapacity = 12,
            DurationMinutes = 45,
            ClubLocationId = locations[2].Id,
        },
    ];

    private static List<ClassSchedule> SeedSchedules(List<FitnessClass> classes, List<Trainer> trainers)
    {
        var mondayNext = NextWeekday(DayOfWeek.Monday);

        return
        [
            new ClassSchedule
            {
                Id = Guid.Parse("66666666-0000-0000-0000-000000000001"),
                FitnessClassId = classes[0].Id,
                TrainerId = trainers[0].Id,
                StartsAtUtc = mondayNext.AddHours(13), // 6am Phoenix (UTC-7)
                EndsAtUtc = mondayNext.AddHours(13.75),
                Capacity = classes[0].DefaultCapacity,
            },
            new ClassSchedule
            {
                Id = Guid.Parse("66666666-0000-0000-0000-000000000002"),
                FitnessClassId = classes[1].Id,
                TrainerId = trainers[1].Id,
                StartsAtUtc = mondayNext.AddDays(2).AddHours(16),
                EndsAtUtc = mondayNext.AddDays(2).AddHours(17),
                Capacity = classes[1].DefaultCapacity,
            },
            new ClassSchedule
            {
                Id = Guid.Parse("66666666-0000-0000-0000-000000000003"),
                FitnessClassId = classes[2].Id,
                TrainerId = trainers[2].Id,
                StartsAtUtc = mondayNext.AddDays(4).AddHours(16),
                EndsAtUtc = mondayNext.AddDays(4).AddHours(16.75),
                Capacity = classes[2].DefaultCapacity,
            },
        ];
    }

    private static List<Member> SeedMembers(List<ClubLocation> locations) =>
    [
        new()
        {
            Id = Guid.Parse("77777777-0000-0000-0000-000000000001"),
            FirstName = "Jordan",
            LastName = "Blake",
            Email = "jordan.blake@example.com",
            PhoneNumber = "480-555-0201",
            DateOfBirth = new DateOnly(1994, 3, 12),
            Status = MemberStatus.Active,
            HomeLocationId = locations[0].Id,
        },
        new()
        {
            Id = Guid.Parse("77777777-0000-0000-0000-000000000002"),
            FirstName = "Priya",
            LastName = "Natarajan",
            Email = "priya.natarajan@example.com",
            PhoneNumber = "480-555-0202",
            DateOfBirth = new DateOnly(1989, 7, 25),
            Status = MemberStatus.Active,
            HomeLocationId = locations[1].Id,
        },
        new()
        {
            Id = Guid.Parse("77777777-0000-0000-0000-000000000003"),
            FirstName = "Sam",
            LastName = "Alvarez",
            Email = "sam.alvarez@example.com",
            PhoneNumber = "480-555-0203",
            DateOfBirth = new DateOnly(2001, 11, 2),
            Status = MemberStatus.PendingActivation,
            HomeLocationId = locations[2].Id,
        },
    ];

    private static List<Membership> SeedMemberships(List<Member> members, List<MembershipPlan> plans)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return
        [
            new Membership
            {
                MemberId = members[0].Id,
                MembershipPlanId = plans[1].Id, // Premium Monthly
                StartDate = today.AddMonths(-1),
                EndDate = today.AddMonths(1),
                Status = MembershipStatus.Active,
            },
            new Membership
            {
                MemberId = members[1].Id,
                MembershipPlanId = plans[2].Id, // Annual
                StartDate = today.AddMonths(-6),
                EndDate = today.AddMonths(6),
                Status = MembershipStatus.Active,
            },
        ];
    }

    private static List<ApplicationUser> SeedUsers(PasswordHasher hasher, List<Member> members, List<Trainer> trainers) =>
    [
        new()
        {
            Id = Guid.Parse("88888888-0000-0000-0000-000000000001"),
            Email = "admin@westcoastfitness.example",
            PasswordHash = hasher.Hash("ChangeMe123!"),
            Role = UserRole.Admin,
        },
        new()
        {
            Id = Guid.Parse("88888888-0000-0000-0000-000000000002"),
            Email = members[0].Email,
            PasswordHash = hasher.Hash("ChangeMe123!"),
            Role = UserRole.Member,
            MemberId = members[0].Id,
        },
        new()
        {
            Id = Guid.Parse("88888888-0000-0000-0000-000000000003"),
            Email = trainers[0].Email,
            PasswordHash = hasher.Hash("ChangeMe123!"),
            Role = UserRole.Trainer,
            TrainerId = trainers[0].Id,
        },
    ];

    private static DateTime NextWeekday(DayOfWeek dayOfWeek)
    {
        var today = DateTime.UtcNow.Date;
        var daysUntil = ((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7;
        daysUntil = daysUntil == 0 ? 7 : daysUntil;
        return today.AddDays(daysUntil);
    }
}
