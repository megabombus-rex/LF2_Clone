namespace LF2Clone.Misc.Logger
{
    public class RaylibLoggerWrapper
    {
        public RaylibLoggerWrapper(ILogger logger)
        {
            _logger = logger;
        }

        private ILogger _logger;

        public void LogRay(int logType, string message)
        {
            var logLevel = logType switch
            {
                0 => ILogger.LogLevel.Trace,
                _ => ILogger.LogLevel.Info,
            };

            switch (logLevel)
            {
                case ILogger.LogLevel.Trace:
                    _logger.LogTrace(message);
                    break;
                case ILogger.LogLevel.Debug:
                    _logger.LogDebug(message);
                    break;
                case ILogger.LogLevel.Info:
                    _logger.LogInfo(message);
                    break;
                case ILogger.LogLevel.Warning:
                    _logger.LogWarning(message); 
                    break;    
                case ILogger.LogLevel.Error:
                    _logger.LogError(message); 
                    break;
            }
        }
    }
}
