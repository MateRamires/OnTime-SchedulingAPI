namespace OnTimeScheduling.Exceptions.ExceptionBase;

public class ErrorOnValidationException : OnTimeSchedulingException
{
    public IReadOnlyCollection<string> ErrorsMessages { get; }

    public ErrorOnValidationException(IEnumerable<string> errorsMessages) : base("Validation error.")
    {
        ErrorsMessages = errorsMessages?
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToArray()
            ?? Array.Empty<string>();
    }
}
