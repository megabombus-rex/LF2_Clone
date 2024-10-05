namespace LF2Clone.Exceptions
{
    [Serializable]
    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException() { }
        public ConfigurationNotFoundException(string message) : base(message) { }
        public ConfigurationNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
