using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Appointments;

public class RegisterAppointmentValidator : AbstractValidator<RequestRegisterAppointmentJson>
{
    public RegisterAppointmentValidator() 
    {
        RuleFor(a => a.ProfessionalId).NotEmpty().WithMessage("Professional ID is required.");
        RuleFor(a => a.ServiceId).NotEmpty().WithMessage("Service ID is required.");
        RuleFor(a => a.LocationId).NotEmpty().WithMessage("Location ID is required.");

        RuleFor(a => a.StartTime)
            .Must(startTime => startTime > DateTime.UtcNow)
            .WithMessage("Appointments cannot be scheduled in the past.");

        RuleFor(a => a.StartTime)
            .Must(startTime => startTime.Kind == DateTimeKind.Utc)
            .WithMessage("StartTime must be in UTC (ISO-8601 with 'Z').");

    }
}
