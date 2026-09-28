using FluentValidation;
using WestCoastFitness.Application.Scheduling.Dtos;

namespace WestCoastFitness.Application.Scheduling.Validators;

public class RegisterForClassRequestValidator : AbstractValidator<RegisterForClassRequest>
{
    public RegisterForClassRequestValidator()
    {
        RuleFor(x => x.ClassScheduleId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
    }
}

public class CreateClassScheduleRequestValidator : AbstractValidator<CreateClassScheduleRequest>
{
    public CreateClassScheduleRequestValidator()
    {
        RuleFor(x => x.FitnessClassId).NotEmpty();
        RuleFor(x => x.TrainerId).NotEmpty();
        RuleFor(x => x.Capacity).GreaterThan(0).LessThanOrEqualTo(200);
        RuleFor(x => x.EndsAtUtc).GreaterThan(x => x.StartsAtUtc);
    }
}
