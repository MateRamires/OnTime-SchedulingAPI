using FluentValidation;

namespace OnTimeScheduling.Application.Validators.Users;

public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("The Name is Required")
            .MinimumLength(3).WithMessage("The Name must be at least 3 characters long")
            .MaximumLength(150).WithMessage("The Name must have less than 150 characters");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("The Email is Required")
            .EmailAddress().WithMessage("The Email is not valid")
            .MaximumLength(200).WithMessage("The Email must have less than 200 characters");

        RuleFor(user => user.Role)
            .Must(role => role is UserRole.COMPANY_ADMIN or UserRole.ATTENDANT or UserRole.PROVIDER)
            .WithMessage("Only CompanyAdmin, Attendant or Provider roles are allowed for company users.");
    }

}
