namespace OnTimeScheduling.Exceptions.ExceptionBase;

public class ConflictException : OnTimeSchedulingException
{
    public ConflictException(string message) : base(message)
    {
    }
}
