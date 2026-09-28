using WestCoastFitness.Domain.Enums;

namespace WestCoastFitness.Application.Scheduling.Dtos;

public record RegisterForClassRequest(Guid ClassScheduleId, Guid MemberId);

public record CancelRegistrationRequest(Guid ClassRegistrationId);

public record ClassRegistrationResultDto(
    Guid RegistrationId,
    Guid ClassScheduleId,
    Guid MemberId,
    RegistrationStatus Status,
    int? WaitlistPosition);

public record CreateClassScheduleRequest(
    Guid FitnessClassId,
    Guid TrainerId,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    int Capacity);
