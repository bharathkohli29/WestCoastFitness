namespace WestCoastFitness.Domain.Enums;

public enum UserRole
{
    Member = 0,
    Trainer = 1,
    FrontDesk = 2,
    Manager = 3,
    Admin = 4,
}

public enum MemberStatus
{
    PendingActivation = 0,
    Active = 1,
    Suspended = 2,
    Cancelled = 3,
}

public enum MembershipStatus
{
    Pending = 0,
    Active = 1,
    Expired = 2,
    Cancelled = 3,
}

public enum ClassCategory
{
    Cardio = 0,
    Strength = 1,
    Yoga = 2,
    Cycling = 3,
    Hiit = 4,
    Aquatics = 5,
    Recovery = 6,
}

public enum ScheduleStatus
{
    Scheduled = 0,
    Cancelled = 1,
    Completed = 2,
}

public enum RegistrationStatus
{
    Registered = 0,
    Waitlisted = 1,
    CancelledByMember = 2,
    CancelledByClub = 3,
    Attended = 4,
    NoShow = 5,
}

public enum GoalType
{
    WeightLoss = 0,
    MuscleGain = 1,
    Endurance = 2,
    Flexibility = 3,
    General = 4,
}

public enum GoalStatus
{
    InProgress = 0,
    Achieved = 1,
    Abandoned = 2,
}

public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    BankTransfer = 2,
    Cash = 3,
}

public enum PaymentStatus
{
    Pending = 0,
    Succeeded = 1,
    Failed = 2,
    Refunded = 3,
}

public enum InvoiceStatus
{
    Draft = 0,
    Issued = 1,
    Paid = 2,
    Overdue = 3,
    Void = 4,
}

public enum DiscountType
{
    Percentage = 0,
    FixedAmount = 1,
}

public enum NotificationType
{
    ClassReminder = 0,
    WaitlistPromotion = 1,
    PaymentReceipt = 2,
    MembershipExpiring = 3,
    General = 4,
}
