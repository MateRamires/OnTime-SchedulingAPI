using FluentValidation;
using OnTimeScheduling.Application.Validators.Password;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Users;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
    public RegisterUserValidator() 
    {
        RuleFor(user => user.Name)
             .NotEmpty().WithMessage("Name Cannot be Empty!")
             .MinimumLength(3).WithMessage("Name must have at least 3 characters.")
             .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email Cannot be Empty!");

        When(user => !string.IsNullOrEmpty(user.Email), () =>
        {
            RuleFor(user => user.Email)
                .EmailAddress().WithMessage("Email is Invalid!")
                .Must(email => !email.Contains(" ")).WithMessage("Email cannot contain spaces!")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");
        });

        RuleFor(user => user.Password)
            .SetValidator(new PasswordValidator<RequestRegisterUserJson>());

        RuleFor(user => user.Role)
            .IsInEnum().WithMessage("Role is Invalid!");
    }
}
