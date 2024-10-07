namespace LF2Clone.Exceptions
{
    [Serializable]
    public class SceneNotLoadedException : Exception
    {
        public SceneNotLoadedException() { }
        public SceneNotLoadedException(string message) : base(message) { }
        public SceneNotLoadedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
