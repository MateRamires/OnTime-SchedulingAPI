namespace OnTimeScheduling.Exceptions.ExceptionBase;

public class ErrorOnUnauthorizedException : OnTimeSchedulingException
{
    public ErrorOnUnauthorizedException(string message) : base(message) { }
}
