namespace Graph.Exceptions;

public class InvalidGraphException : Exception
{
    public InvalidGraphException(string message)
        : base(message)
    {
    }
}