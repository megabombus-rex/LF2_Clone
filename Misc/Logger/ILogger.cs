namespace LF2Clone.Misc.Logger
{
    public interface ILogger
    {
        LogLevel LoggingLevel { get; set; }

        void LogError(string message);
        void LogWarning(string message);
        void LogInfo(string message);
        void LogDebug(string message);

        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }
    }
}