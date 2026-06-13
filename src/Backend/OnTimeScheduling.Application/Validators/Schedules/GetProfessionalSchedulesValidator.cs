using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Schedules;

public class GetProfessionalSchedulesValidator : AbstractValidator<RequestGetProfessionalSchedulesJson>
{
    public GetProfessionalSchedulesValidator()
    {
        RuleFor(s => s.DayOfWeek)
            .Must(dayOfWeek => !dayOfWeek.HasValue || Enum.IsDefined(dayOfWeek.Value))
            .WithMessage("Invalid Day of Week.");
    }
}
