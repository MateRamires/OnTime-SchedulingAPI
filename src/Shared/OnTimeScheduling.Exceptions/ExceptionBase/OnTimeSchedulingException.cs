namespace OnTimeScheduling.Exceptions.ExceptionBase;

public abstract class OnTimeSchedulingException : Exception
{
    protected OnTimeSchedulingException(string message) : base(message) { }
}
