using Microsoft.AspNetCore.RateLimiting;
using OnTimeScheduling.Communication.Responses;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace OnTimeScheduling.Api.RateLimiting;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitingOptions = configuration
            .GetSection(ApiRateLimitingOptions.SectionName)
            .Get<ApiRateLimitingOptions>() ?? new ApiRateLimitingOptions();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: BuildPartitionKey(httpContext, "global"),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitingOptions.Global.PermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimitingOptions.Global.WindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy(RateLimitingPolicyNames.AuthStrict, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: BuildIpPartitionKey(httpContext, RateLimitingPolicyNames.AuthStrict),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitingOptions.AuthStrict.PermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimitingOptions.AuthStrict.WindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy(RateLimitingPolicyNames.ScheduleRead, httpContext =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: BuildPartitionKey(httpContext, RateLimitingPolicyNames.ScheduleRead),
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitingOptions.ScheduleRead.PermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimitingOptions.ScheduleRead.WindowSeconds),
                        SegmentsPerWindow = rateLimitingOptions.ScheduleRead.SegmentsPerWindow,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                var httpContext = context.HttpContext;
                var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
                var policyName = ResolvePolicyName(httpContext);
                var limit = ResolvePermitLimit(rateLimitingOptions, policyName);

                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.Headers["RateLimit-Policy"] = policyName;
                httpContext.Response.Headers["RateLimit-Limit"] = limit.ToString();
                httpContext.Response.Headers["RateLimit-Remaining"] = "0";

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    var retryAfterSeconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
                    httpContext.Response.Headers["Retry-After"] = retryAfterSeconds.ToString();
                    httpContext.Response.Headers["RateLimit-Reset"] = retryAfterSeconds.ToString();
                }

                var response = new ResponseErrorJson(
                    "Too many requests. Please wait before trying again.",
                    traceId);

                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            };
        });

        return services;
    }

    private static string BuildPartitionKey(HttpContext httpContext, string policyName)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.Sid);
        var companyId = httpContext.User.FindFirstValue("CompanyId");

        if (!string.IsNullOrWhiteSpace(userId))
        {
            return string.IsNullOrWhiteSpace(companyId)
                ? $"{policyName}:user:{userId}"
                : $"{policyName}:company:{companyId}:user:{userId}";
        }

        return BuildIpPartitionKey(httpContext, policyName);
    }

    private static string BuildIpPartitionKey(HttpContext httpContext, string policyName)
    {
        return $"{policyName}:ip:{GetClientIp(httpContext)}";
    }

    private static string GetClientIp(HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static string ResolvePolicyName(HttpContext httpContext)
    {
        var endpointPolicy = httpContext.GetEndpoint()
            ?.Metadata
            .GetMetadata<EnableRateLimitingAttribute>()
            ?.PolicyName;

        return string.IsNullOrWhiteSpace(endpointPolicy)
            ? "global"
            : endpointPolicy;
    }

    private static int ResolvePermitLimit(ApiRateLimitingOptions options, string policyName)
    {
        return policyName switch
        {
            RateLimitingPolicyNames.AuthStrict => options.AuthStrict.PermitLimit,
            RateLimitingPolicyNames.ScheduleRead => options.ScheduleRead.PermitLimit,
            _ => options.Global.PermitLimit
        };
    }

}
