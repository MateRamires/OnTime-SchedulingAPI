using FluentValidation;

namespace OnTimeScheduling.Application.Validators.ScheduleBlocks;

public class UpdateScheduleBlockValidator : AbstractValidator<RequestUpdateScheduleBlockJson>
{
    public UpdateScheduleBlockValidator()
    {
        RuleFor(b => b)
            .Must(b => b.ProfessionalId.HasValue || b.LocationId.HasValue)
            .WithMessage("A schedule block must target a professional, a location, or both.");

        RuleFor(b => b.StartTime)
            .Must(startTime => startTime.Kind == DateTimeKind.Utc)
            .WithMessage("StartTime must be in UTC (ISO-8601 with 'Z').");

        RuleFor(b => b.EndTime)
            .Must(endTime => endTime.Kind == DateTimeKind.Utc)
            .WithMessage("EndTime must be in UTC (ISO-8601 with 'Z').");

        RuleFor(b => b.StartTime)
            .LessThan(b => b.EndTime)
            .WithMessage("The schedule block start time must be before the end time.");

        RuleFor(b => b.EndTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Schedule blocks must end in the future.");

        RuleFor(b => b.Reason)
            .MaximumLength(500)
            .WithMessage("Reason must be at most 500 characters.");
    }

}
