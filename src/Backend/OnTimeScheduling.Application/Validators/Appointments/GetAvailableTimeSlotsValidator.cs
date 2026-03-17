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

        // A TargetDate pode ser hoje, ou no futuro. Não permitimos buscar agenda de ontem.
        // Convertendo ambos para Date (ignorando a hora) para fazer a comparação correta.
        RuleFor(x => x.TargetDate.Date)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Cannot search for available slots in the past.");
    }
}
