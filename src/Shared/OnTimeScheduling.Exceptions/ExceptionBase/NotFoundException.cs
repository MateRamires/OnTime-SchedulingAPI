namespace OnTimeScheduling.Exceptions.ExceptionBase;

public class NotFoundException : OnTimeSchedulingException
{
    public NotFoundException(string message) : base(message) { }
}
