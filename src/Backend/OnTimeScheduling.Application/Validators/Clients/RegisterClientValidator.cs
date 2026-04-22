using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Clients;

public class RegisterClientValidator : AbstractValidator<RequestRegisterClientJson>
{
    public RegisterClientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .MaximumLength(120)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }

}
