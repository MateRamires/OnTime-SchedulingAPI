using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Locations;

public class RegisterLocationValidator : AbstractValidator<RequestRegisterLocationJson>
{
    public RegisterLocationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The location name is required.")
            .MinimumLength(3).WithMessage("The name must be at least 3 characters long.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("The address is required.");
    }
}
