using FluentValidation;
using OnTimeScheduling.Application.Validators.Password;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Users;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
    public RegisterUserValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage("Name Cannot be Empty!");
        RuleFor(user => user.Email).NotEmpty().WithMessage("Email Cannot be Empty!");
        RuleFor(user => user.Password).SetValidator(new PasswordValidator<RequestRegisterUserJson>());
        RuleFor(user => user.Role).IsInEnum().WithMessage("Role is Invalid!");
        When(user => !string.IsNullOrEmpty(user.Email), () =>
        {
            RuleFor(user => user.Email).EmailAddress().WithMessage("Email is Invalid!");
        });
    }
}
