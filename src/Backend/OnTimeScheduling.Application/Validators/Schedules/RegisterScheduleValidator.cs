using FluentValidation;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.Validators.Schedules;

public class RegisterScheduleValidator : AbstractValidator<RequestRegisterScheduleJson>
{
    public RegisterScheduleValidator()
    {
        RuleFor(s => s.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(s => s.LocationId)
            .NotEmpty().WithMessage("Location ID is required.");

        RuleFor(s => s.DayOfWeek)
            .IsInEnum().WithMessage("Invalid Day of Week.");

        RuleFor(s => s.StartTime)
            .LessThan(s => s.EndTime).WithMessage("The Start Time must be strictly before the End Time.");

        RuleFor(s => s.StartTime.TotalHours)
            .GreaterThanOrEqualTo(0).WithMessage("Invalid Start Time format.")
            .LessThan(24).WithMessage("Invalid Start Time format.");

        RuleFor(s => s.EndTime.TotalHours)
            .GreaterThan(0).WithMessage("Invalid End Time format.")
            .LessThanOrEqualTo(24).WithMessage("Invalid End Time format.");
    }
}
