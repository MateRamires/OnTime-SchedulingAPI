using FluentValidation;

namespace OnTimeScheduling.Application.Validators.ScheduleBlocks;

public class GetScheduleBlocksValidator : AbstractValidator<RequestGetScheduleBlocksJson>
{
    public GetScheduleBlocksValidator()
    {
        RuleFor(b => b.StartTime)
            .Must(startTime => !startTime.HasValue || startTime.Value.Kind == DateTimeKind.Utc)
            .WithMessage("StartTime must be in UTC (ISO-8601 with 'Z').");

        RuleFor(b => b.EndTime)
            .Must(endTime => !endTime.HasValue || endTime.Value.Kind == DateTimeKind.Utc)
            .WithMessage("EndTime must be in UTC (ISO-8601 with 'Z').");

        RuleFor(b => b)
            .Must(b => !b.StartTime.HasValue || !b.EndTime.HasValue || b.StartTime.Value < b.EndTime.Value)
            .WithMessage("StartTime must be before EndTime.");
    }

}
