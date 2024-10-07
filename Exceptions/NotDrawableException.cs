namespace LF2Clone.Exceptions
{
    public class NotDrawableException : Exception
    {
        public NotDrawableException() { }
        public NotDrawableException(string message) : base(message) { }
        public NotDrawableException(string message, Exception innerException) : base(message, innerException) { }
    }
}
