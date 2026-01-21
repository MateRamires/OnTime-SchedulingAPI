namespace OnTimeScheduling.Exceptions.ExceptionBase;

public class InvalidLoginException : OnTimeSchedulingException
{
    public InvalidLoginException(string message) : base(message) { }
}
