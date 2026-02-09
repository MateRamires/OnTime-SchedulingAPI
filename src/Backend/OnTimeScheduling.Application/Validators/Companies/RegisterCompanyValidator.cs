using FluentValidation;
using OnTimeScheduling.Application.Validators.Password;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Companies;

public class RegisterCompanyValidator : AbstractValidator<RequestRegisterCompanyJson>
{
    public RegisterCompanyValidator()
    {
        RuleFor(c => c.SocialReason).NotEmpty().WithMessage("Social Reason cannot be empty");
        RuleFor(c => c.FantasyName).NotEmpty().WithMessage("Fantasy Name cannot be empty");
        RuleFor(c => c.Phone).NotEmpty().WithMessage("Phone cannot be empty");
        RuleFor(c => c.CNPJ).NotEmpty().WithMessage("CNPJ cannot be empty");
        //TODO: Validate CNPJ function

        RuleFor(c => c.Name).NotEmpty().WithMessage("Admin Name cannot be empty");

        RuleFor(c => c.Email).NotEmpty().WithMessage("Email Cannot be Empty!");

        When(c => !string.IsNullOrEmpty(c.Email), () =>
        {
            RuleFor(c => c.Email)
               .EmailAddress().WithMessage("Email is Invalid!")
               .Must(email => !email.Contains(" ")).WithMessage("Email cannot contain spaces!");
        });

        RuleFor(c => c.Password).SetValidator(new PasswordValidator<RequestRegisterCompanyJson>());
    }
}
