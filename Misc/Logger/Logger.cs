namespace LF2Clone.Misc.Logger
{
    public sealed class Logger<T> : ILogger
    {
        private ConsoleColor ErrorForegroundColor = ConsoleColor.Red;
        private ConsoleColor ErrorBackgroundColor = ConsoleColor.Black;
        private ConsoleColor InfoForegroundColor = ConsoleColor.Cyan;
        private ConsoleColor InfoBackgroundColor = ConsoleColor.Black;
        private ConsoleColor DebugForegroundColor = ConsoleColor.Green;
        private ConsoleColor DebugBackgroundColor = ConsoleColor.Black;
        private ConsoleColor WarningForegroundColor = ConsoleColor.Yellow;
        private ConsoleColor WarningBackgroundColor = ConsoleColor.Black;
        private ConsoleColor TraceForegroundColor = ConsoleColor.DarkYellow;
        private ConsoleColor TraceBackgroundColor = ConsoleColor.Black;
        private ConsoleColor DefaultForegroundColor = ConsoleColor.White;
        private ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;

        public ILogger.LogLevel _loggingLevel = ILogger.LogLevel.Info;

        public ILogger.LogLevel LoggingLevel { get => _loggingLevel; set => _loggingLevel = value; }
        private string _loggingFilePath { get; set; }
        private StreamWriter _writer;

        public void Dispose()
        {
            _writer.Close();
            _writer.Dispose();
        }

        public Logger(string loggingFilePath)
        {
            var loggingFile = string.Format("{0}\\{1}\\LF2C-{2}.log", Environment.CurrentDirectory, loggingFilePath, DateTime.UtcNow.ToString("d"));
            _loggingFilePath = loggingFile;

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

        public void LogTrace(string message)
        {
            if (CheckIfShouldLog(ILogger.LogLevel.Trace)) {  return; }

            Log(TraceForegroundColor, TraceBackgroundColor, ILogger.LogLevel.Trace, message);
        }

        public void ParseAndSetLoggingLevel(string value)
        {
            _loggingLevel = value switch
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
