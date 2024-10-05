namespace LF2Clone.Exceptions
{
    public class NodeReparentingException : Exception
    {
        public NodeReparentingException() { }
        public NodeReparentingException(string message) : base(message) { }
        public NodeReparentingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
