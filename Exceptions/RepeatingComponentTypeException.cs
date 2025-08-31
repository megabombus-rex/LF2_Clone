namespace LF2Clone.Exceptions
{
    public class RepeatingComponentTypeException : Exception
    {
        public RepeatingComponentTypeException() { }
        public RepeatingComponentTypeException(string message) : base(message) { }
        public RepeatingComponentTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
