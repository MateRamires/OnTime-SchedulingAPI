using FluentValidation;
using OnTimeScheduling.Communication.Requests.Appointments;

namespace OnTimeScheduling.Application.Validators.Appointments;

public class GetDailyAgendaValidator : AbstractValidator<RequestGetDailyAgendaJson>
{
    public GetDailyAgendaValidator()
    {
        RuleFor(x => x.Date)
            .NotEqual(default(DateOnly))
            .WithMessage("Date is required.");

        RuleFor(x => x.ProfessionalId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Professional ID cannot be empty.");

        RuleFor(x => x.LocationId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Location ID cannot be empty.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status is invalid.");
    }

}
