namespace LF2Clone.Misc.Logger
{
    public sealed class Logger : ILogger
    {
        public ConsoleColor ErrorForegroundColor = ConsoleColor.Red;
        public ConsoleColor ErrorBackgroundColor = ConsoleColor.Black;
        public ConsoleColor InfoForegroundColor = ConsoleColor.Cyan;
        public ConsoleColor InfoBackgroundColor = ConsoleColor.Black;
        public ConsoleColor DebugForegroundColor = ConsoleColor.Green;
        public ConsoleColor DebugBackgroundColor = ConsoleColor.Black;
        public ConsoleColor WarningForegroundColor = ConsoleColor.Yellow;
        public ConsoleColor WarningBackgroundColor = ConsoleColor.Black;
        public ConsoleColor DefaultForegroundColor = ConsoleColor.White;
        public ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;

        public ILogger.LogLevel _loggingLevel = ILogger.LogLevel.Info;

        public ILogger.LogLevel LoggingLevel { get => _loggingLevel; set => _loggingLevel = value; }

        public Logger()
        {
            
        }

        public Logger(ConsoleColor errorForegroundColor, ConsoleColor errorBackgroundColor, ConsoleColor infoForegroundColor, ConsoleColor infoBackgroundColor, ConsoleColor debugForegroundColor, ConsoleColor debugBackgroundColor, ConsoleColor warningForegroundColor, ConsoleColor warningBackgroundColor, ILogger.LogLevel loggingLevel)
        {
            ErrorForegroundColor = errorForegroundColor;
            ErrorBackgroundColor = errorBackgroundColor;
            InfoForegroundColor = infoForegroundColor;
            InfoBackgroundColor = infoBackgroundColor;
            DebugForegroundColor = debugForegroundColor;
            DebugBackgroundColor = debugBackgroundColor;
            WarningForegroundColor = warningForegroundColor;
            WarningBackgroundColor = warningBackgroundColor;
            _loggingLevel = loggingLevel;
        }

        private void Log(ConsoleColor foreground, ConsoleColor background, ILogger.LogLevel logLevel, string message)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            Console.WriteLine(string.Format("[{0}] {1}: {2}",DateTime.UtcNow.ToString("G"), logLevel.ToString(), message));

            Console.ForegroundColor = DefaultForegroundColor; 
            Console.BackgroundColor = DefaultBackgroundColor;
        }

        private bool CheckIfShouldLog(ILogger.LogLevel logLevel)
        {
            return (int)_loggingLevel > (int)logLevel;
        }

        public void LogError(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Error)) { return; }

            Log(ErrorForegroundColor, ErrorBackgroundColor, ILogger.LogLevel.Error, message);
        }
        public void LogWarning(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Warning)) { return; }

            Log(WarningForegroundColor, WarningBackgroundColor, ILogger.LogLevel.Warning, message);
        }

        public void LogInfo(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Info)) { return; }

            Log(InfoForegroundColor, InfoBackgroundColor, ILogger.LogLevel.Info, message);
        }

        public void LogDebug(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Debug)) { return; }

            Log(DebugForegroundColor, DebugBackgroundColor, ILogger.LogLevel.Debug, message);
        }
    }
}
