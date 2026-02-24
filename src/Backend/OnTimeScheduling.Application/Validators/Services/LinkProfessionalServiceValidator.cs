using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Services;

public class LinkProfessionalServiceValidator : AbstractValidator<RequestLinkProfessionalServiceJson>
{
    public LinkProfessionalServiceValidator() 
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("Service ID is required.");
    }
}
