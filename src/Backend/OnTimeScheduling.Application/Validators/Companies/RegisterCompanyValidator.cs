using FluentValidation;
using OnTimeScheduling.Application.Extensions;
using OnTimeScheduling.Application.Validators.Password;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Companies;

public class RegisterCompanyValidator : AbstractValidator<RequestRegisterCompanyJson>
{
    public RegisterCompanyValidator()
    {
        RuleFor(c => c.SocialReason)
            .NotEmpty().WithMessage("Social Reason is required.")
            .MinimumLength(5).WithMessage("Social Reason must have at least 5 characters.")
            .MaximumLength(200).WithMessage("Social Reason must have less than 200 characters.");

        RuleFor(c => c.FantasyName)
            .NotEmpty().WithMessage("Fantasy Name is required.")
            .MinimumLength(2).WithMessage("Fantasy Name must have at least 2 characters.")
            .MaximumLength(100).WithMessage("Fantasy Name must have less than 100 characters.");

        RuleFor(c => c.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .IsValidPhone(); 

        RuleFor(c => c.CNPJ)
            .NotEmpty().WithMessage("CNPJ is required.")
            .IsValidCNPJ();

        RuleFor(c => c.CompanyEmail).NotEmpty().WithMessage("Company Email is required.");
        When(c => !string.IsNullOrEmpty(c.CompanyEmail), () =>
        {
            RuleFor(c => c.CompanyEmail)
                .EmailAddress().WithMessage("Company Email is invalid.")
                .MaximumLength(200);
        });

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Admin Name is required.")
            .MinimumLength(3).WithMessage("Admin Name must have at least 3 characters.")
            .MaximumLength(100).WithMessage("Admin Name must have less than 100 characters.");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required.");

        When(c => !string.IsNullOrEmpty(c.Email), () =>
        {
            RuleFor(c => c.Email)
                .EmailAddress().WithMessage("Email is invalid.")
                .Must(email => !email.Contains(" ")).WithMessage("Email cannot contain spaces.")
                .MaximumLength(200).WithMessage("Email must have less than 200 characters.");
        });

        RuleFor(c => c.Password).SetValidator(new PasswordValidator<RequestRegisterCompanyJson>());
    }
}
