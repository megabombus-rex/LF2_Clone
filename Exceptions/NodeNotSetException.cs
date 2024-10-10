namespace LF2Clone.Exceptions
{
    [Serializable]
    public class NodeNotSetException : Exception
    {
        public NodeNotSetException() { }
        public NodeNotSetException(string message) : base(message) { }
        public NodeNotSetException(string message, Exception innerException) : base(message, innerException) { }
    }
}
