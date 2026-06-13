using FluentValidation;
using OnTimeScheduling.Communication.Requests.Appointments;

namespace OnTimeScheduling.Application.Validators.Appointments;

public class GetMyAgendaValidator : AbstractValidator<RequestGetMyAgendaJson>
{
    public GetMyAgendaValidator()
    {
        RuleFor(x => x.Date)
            .NotEqual(default(DateOnly))
            .WithMessage("Date is required.");

        RuleFor(x => x.Window)
            .IsInEnum()
            .WithMessage("Window is invalid.");

        RuleFor(x => x.LocationId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Location ID cannot be empty.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status is invalid.");
    }

}
