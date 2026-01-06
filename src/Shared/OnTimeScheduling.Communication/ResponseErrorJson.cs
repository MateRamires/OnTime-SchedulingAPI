namespace OnTimeScheduling.Communication;

public class ResponseErrorJson
{
    public IList<string> Errors { get; } = new List<string>();
    public string TraceId { get; } //TODO: Show traceId only in Dev, not in production.

    public ResponseErrorJson(string error, string traceId = "")
    {
        if (!string.IsNullOrWhiteSpace(error))
            Errors.Add(error);

        TraceId = traceId;
    }

    public ResponseErrorJson(IList<string> errors, string traceId = "")
    {
        if (errors is not null)
        {
            foreach (var e in errors.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
                Errors.Add(e);
        }

        TraceId = traceId;
    }
}
