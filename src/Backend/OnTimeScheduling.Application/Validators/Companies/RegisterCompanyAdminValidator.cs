using FluentValidation;
using OnTimeScheduling.Application.Validators.Password;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Companies;

public class RegisterCompanyAdminValidator : AbstractValidator<RequestRegisterCompanyAdminJson>
{
    public RegisterCompanyAdminValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Admin Name is required.")
            .MinimumLength(3).WithMessage("Admin Name must have at least 3 characters.")
            .MaximumLength(150).WithMessage("Admin Name cannot exceed 150 characters.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.");

        When(user => !string.IsNullOrEmpty(user.Email), () =>
        {
            RuleFor(user => user.Email)
                .EmailAddress().WithMessage("Email is invalid.")
                .Must(email => !email.Contains(" ")).WithMessage("Email cannot contain spaces.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");
        });

        RuleFor(user => user.Password)
            .SetValidator(new PasswordValidator<RequestRegisterCompanyAdminJson>());
    }
}
