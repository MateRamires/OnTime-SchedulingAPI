using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Appointments;

public class GetAvailableTimeSlotsValidator : AbstractValidator<RequestGetAvailableTimeSlotsJson>
{
    public GetAvailableTimeSlotsValidator() 
    {
        RuleFor(x => x.ProfessionalId).NotEmpty().WithMessage("Professional ID is required.");
        RuleFor(x => x.LocationId).NotEmpty().WithMessage("Location ID is required.");
        RuleFor(x => x.ServiceId).NotEmpty().WithMessage("Service ID is required.");

        RuleFor(x => x.TargetDate)
            .NotEqual(default(DateOnly))
            .WithMessage("TargetDate is required.");
    }
}
