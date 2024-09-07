namespace LF2Clone.Misc.Logger
{
    public interface ILogger : IDisposable
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogInfo(string message);
        void LogDebug(string message);
        void LogTrace(string message);
        LogLevel ParseLoggingLevel(string value);
        public enum LogLevel
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warning = 3,
            Error = 4
        }
    }
}