namespace LF2Clone.Exceptions
{
    [Serializable]
    public class SceneNotFoundException : Exception
    {
        public SceneNotFoundException() { }
        public SceneNotFoundException(string message) : base(message) { }
        public SceneNotFoundException(string message, Exception innerException) : base(message, innerException) { }

    }
}
