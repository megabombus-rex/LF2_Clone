namespace LF2Clone.Misc.Logger
{
    public sealed class Logger<T> : ILogger
    {
        private ConsoleColor _errorForegroundColor = ConsoleColor.Red;
        private ConsoleColor _errorBackgroundColor = ConsoleColor.Black;
        private ConsoleColor _infoForegroundColor = ConsoleColor.Cyan;
        private ConsoleColor _infoBackgroundColor = ConsoleColor.Black;
        private ConsoleColor _debugForegroundColor = ConsoleColor.Green;
        private ConsoleColor _debugBackgroundColor = ConsoleColor.Black;
        private ConsoleColor _warningForegroundColor = ConsoleColor.Yellow;
        private ConsoleColor _warningBackgroundColor = ConsoleColor.Black;
        private ConsoleColor _traceForegroundColor = ConsoleColor.DarkYellow;
        private ConsoleColor _traceBackgroundColor = ConsoleColor.Black;
        private ConsoleColor _defaultForegroundColor = ConsoleColor.White;
        private ConsoleColor _defaultBackgroundColor = ConsoleColor.Black;

        private ILogger.LogLevel _loggingLevel = ILogger.LogLevel.Info;
        private StreamWriter _writer;

        public void Dispose()
        {
            _writer.Close();
            _writer.Dispose();
        }

        public Logger(string loggingFilePath, string logLevel)
        {
            var loggingFile = string.Format("{0}\\{1}\\LF2C-{2}.log", Environment.CurrentDirectory, loggingFilePath, DateTime.UtcNow.ToString("d"));

            _loggingLevel = ParseLoggingLevel(logLevel);

            _writer = new StreamWriter(loggingFile, System.Text.Encoding.Default, new FileStreamOptions()
            {
                Access = FileAccess.Write,
                Share = FileShare.ReadWrite,
                Mode = FileMode.Append,
                Options = FileOptions.RandomAccess,
            });
        }

        private void Log(ConsoleColor foreground, ConsoleColor background, ILogger.LogLevel logLevel, string message)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            var finalMessage = string.Format("[{0}] System: {1}. {2}: {3}", DateTime.UtcNow.ToString("G"), typeof(T).Name, logLevel.ToString(), message);
            Console.WriteLine(finalMessage);
            _writer.WriteLine(finalMessage);
            _writer.Flush();

            Console.ForegroundColor = _defaultForegroundColor; 
            Console.BackgroundColor = _defaultBackgroundColor;
        }

        private bool CheckIfShouldLog(ILogger.LogLevel logLevel)
        {
            return (int)_loggingLevel > (int)logLevel;
        }

        public void LogError(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Error)) { return; }

            Log(_errorForegroundColor, _errorBackgroundColor, ILogger.LogLevel.Error, message);
        }
        public void LogWarning(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Warning)) { return; }

            Log(_warningForegroundColor, _warningBackgroundColor, ILogger.LogLevel.Warning, message);
        }

        public void LogInfo(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Info)) { return; }

            Log(_infoForegroundColor, _infoBackgroundColor, ILogger.LogLevel.Info, message);
        }

        public void LogDebug(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Debug)) { return; }

            Log(_debugForegroundColor, _debugBackgroundColor, ILogger.LogLevel.Debug, message);
        }

        public void LogTrace(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Trace)) {  return; }

            Log(_traceForegroundColor, _traceBackgroundColor, ILogger.LogLevel.Trace, message);
        }

        public ILogger.LogLevel ParseLoggingLevel(string value)
        {
            return value switch
            {
                "Trace" => ILogger.LogLevel.Trace,
                "Debug" => ILogger.LogLevel.Debug,
                "Info" => ILogger.LogLevel.Info,
                "Warning" => ILogger.LogLevel.Warning,
                "Error" => ILogger.LogLevel.Error,
                _ => ILogger.LogLevel.Info,
            };
        }
    }
}
