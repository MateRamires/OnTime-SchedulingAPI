using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Locations;

public class RegisterLocationValidator : AbstractValidator<RequestRegisterLocationJson>
{
    public RegisterLocationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The location name is required.")
            .MinimumLength(3).WithMessage("The name must be at least 3 characters long.")
            .MaximumLength(100).WithMessage("The name must have less than 100 characters.");


        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("The address is required.")
            .MaximumLength(250).WithMessage("The address must have less than 250 characters.");

        RuleFor(x => x.TimeZoneId)
            .MaximumLength(100).WithMessage("The timezone must have less than 100 characters.");
    }
}
