namespace LF2Clone.Misc.Logger
{
    public sealed class Logger<T> : ILogger<T>
    {
        private ConsoleColor ErrorForegroundColor = ConsoleColor.Red;
        private ConsoleColor ErrorBackgroundColor = ConsoleColor.Black;
        private ConsoleColor InfoForegroundColor = ConsoleColor.Cyan;
        private ConsoleColor InfoBackgroundColor = ConsoleColor.Black;
        private ConsoleColor DebugForegroundColor = ConsoleColor.Green;
        private ConsoleColor DebugBackgroundColor = ConsoleColor.Black;
        private ConsoleColor WarningForegroundColor = ConsoleColor.Yellow;
        private ConsoleColor WarningBackgroundColor = ConsoleColor.Black;
        private ConsoleColor DefaultForegroundColor = ConsoleColor.White;
        private ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;

        public ILogger<T>.LogLevel _loggingLevel = ILogger<T>.LogLevel.Info;

        public ILogger<T>.LogLevel LoggingLevel { get => _loggingLevel; set => _loggingLevel = value; }

        public Logger()
        {
            ErrorForegroundColor = ConsoleColor.Red;
            ErrorBackgroundColor = ConsoleColor.Black;
            InfoForegroundColor = ConsoleColor.Cyan;
            InfoBackgroundColor = ConsoleColor.Black;
            DebugForegroundColor = ConsoleColor.Green;
            DebugBackgroundColor = ConsoleColor.Black;
            WarningForegroundColor = ConsoleColor.Yellow;
            WarningBackgroundColor = ConsoleColor.Black;
            DefaultForegroundColor = ConsoleColor.White;
            DefaultBackgroundColor = ConsoleColor.Black;
        }

        public Logger(ConsoleColor errorForegroundColor, 
            ConsoleColor errorBackgroundColor, 
            ConsoleColor infoForegroundColor, 
            ConsoleColor infoBackgroundColor, 
            ConsoleColor debugForegroundColor, 
            ConsoleColor debugBackgroundColor, 
            ConsoleColor warningForegroundColor, 
            ConsoleColor warningBackgroundColor, 
            ILogger<T>.LogLevel loggingLevel)
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

        private void Log(ConsoleColor foreground, ConsoleColor background, ILogger<T>.LogLevel logLevel, string message)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            Console.WriteLine(string.Format("[{0}] System: {1}. {2}: {3}",DateTime.UtcNow.ToString("G"), typeof(T).Name, logLevel.ToString(), message));

            Console.ForegroundColor = DefaultForegroundColor; 
            Console.BackgroundColor = DefaultBackgroundColor;
        }

        private bool CheckIfShouldLog(ILogger<T>.LogLevel logLevel)
        {
            return (int)_loggingLevel > (int)logLevel;
        }

        public void LogError(string message)
        {
            if (CheckIfShouldLog(ILogger<T>.LogLevel.Error)) { return; }

            Log(ErrorForegroundColor, ErrorBackgroundColor, ILogger<T>.LogLevel.Error, message);
        }
        public void LogWarning(string message)
        {
            if (CheckIfShouldLog(ILogger<T>.LogLevel.Warning)) { return; }

            Log(WarningForegroundColor, WarningBackgroundColor, ILogger<T>.LogLevel.Warning, message);
        }

        public void LogInfo(string message)
        {
            if (CheckIfShouldLog(ILogger<T>.LogLevel.Info)) { return; }

            Log(InfoForegroundColor, InfoBackgroundColor, ILogger<T>.LogLevel.Info, message);
        }

        public void LogDebug(string message)
        {
            if (CheckIfShouldLog(ILogger<T>.LogLevel.Debug)) { return; }

            Log(DebugForegroundColor, DebugBackgroundColor, ILogger<T>.LogLevel.Debug, message);
        }
    }
}
