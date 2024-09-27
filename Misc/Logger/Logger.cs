namespace LF2Clone.Misc.Logger
{
    public class Logger : ILogger
    {

        protected ConsoleColor _errorForegroundColor = ConsoleColor.Red;
        protected ConsoleColor _errorBackgroundColor = ConsoleColor.Black;
        protected ConsoleColor _infoForegroundColor = ConsoleColor.Cyan;
        protected ConsoleColor _infoBackgroundColor = ConsoleColor.Black;
        protected ConsoleColor _debugForegroundColor = ConsoleColor.Green;
        protected ConsoleColor _debugBackgroundColor = ConsoleColor.Black;
        protected ConsoleColor _warningForegroundColor = ConsoleColor.Yellow;
        protected ConsoleColor _warningBackgroundColor = ConsoleColor.Black;
        protected ConsoleColor _traceForegroundColor = ConsoleColor.DarkYellow;
        protected ConsoleColor _traceBackgroundColor = ConsoleColor.Black;
        protected ConsoleColor _defaultForegroundColor = ConsoleColor.White;
        protected ConsoleColor _defaultBackgroundColor = ConsoleColor.Black;

        protected ILogger.LogLevel _loggingLevel = ILogger.LogLevel.Info;
        protected StreamWriter _writer;

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

        protected virtual void Log(ConsoleColor foreground, ConsoleColor background, ILogger.LogLevel logLevel, string message)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            var finalMessage = string.Format("[{0}] {1}: {2}", DateTime.UtcNow.ToString("G"), logLevel.ToString(), message);
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
            if (CheckIfShouldLog(ILogger.LogLevel.Trace)) { return; }

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

    public sealed class Logger<T> : Logger
    {
        public Logger(string loggingFilePath, string logLevel) : base(loggingFilePath, logLevel)
        {
        }

        protected override void Log(ConsoleColor foreground, ConsoleColor background, ILogger.LogLevel logLevel, string message)
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
    }
}
