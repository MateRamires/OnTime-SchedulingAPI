namespace OnTimeScheduling.Api.RateLimiting;

public class ApiRateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public RateLimitRuleOptions Global { get; init; } = new()
    {
        PermitLimit = 300,
        WindowSeconds = 60
    };

    public RateLimitRuleOptions AuthStrict { get; init; } = new()
    {
        PermitLimit = 5,
        WindowSeconds = 60
    };

    public SlidingWindowRateLimitRuleOptions ScheduleRead { get; init; } = new()
    {
        PermitLimit = 60,
        WindowSeconds = 60,
        SegmentsPerWindow = 6
    };
}

public class RateLimitRuleOptions
{
    public int PermitLimit { get; init; }
    public int WindowSeconds { get; init; }
}

public class SlidingWindowRateLimitRuleOptions : RateLimitRuleOptions
{
    public int SegmentsPerWindow { get; init; }
}
