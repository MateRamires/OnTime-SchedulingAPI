using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Services;

public class RegisterServiceValidator : AbstractValidator<RequestRegisterServiceJson>
{
    public RegisterServiceValidator() 
    {
        RuleFor(s => s.Name)
            .NotEmpty().WithMessage("Service Name is required.")
            .MinimumLength(3).WithMessage("Service Name must have at least 3 characters.")
            .MaximumLength(150).WithMessage("Service Name must have less than 150 characters.");

        When(s => !string.IsNullOrWhiteSpace(s.Description), () =>
        {
            RuleFor(s => s.Description)
                .MaximumLength(500).WithMessage("Description must have less than 500 characters.");
        });

        RuleFor(s => s.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        RuleFor(s => s.DurationInMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than zero.");
    }
}
