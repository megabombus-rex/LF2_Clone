using System.Runtime.InteropServices;

namespace LF2Clone.Misc.Logger
{
    // experimental, not working properly
    public class RaylibLoggerWrapper
    {
        private ILogger _logger;
        public RaylibLoggerWrapper(ILogger logger)
        {
            _logger = logger;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void TraceLogCallback(int logLevel, IntPtr message, IntPtr args);

        private static TraceLogCallback _traceLogCallbackDelegate;

        public void Initialize()
        {
            _traceLogCallbackDelegate = TraceLogCallbackHandler;

            var functionPointer = Marshal.GetFunctionPointerForDelegate(_traceLogCallbackDelegate);

            SetTraceLogCallback(functionPointer);
        }

        private void TraceLogCallbackHandler(int logType, IntPtr message, IntPtr args)
        {
            var logLevel = logType switch
            {
                0 => ILogger.LogLevel.Trace,
                1 => ILogger.LogLevel.Debug,
                2 => ILogger.LogLevel.Info,
                3 => ILogger.LogLevel.Warning,
                4 => ILogger.LogLevel.Error,
                _ => ILogger.LogLevel.Info,
            };

            var newMessage = Raylib_cs.Logging.GetLogMessage(message, args);


            switch (logLevel)
            {
                case ILogger.LogLevel.Trace:
                    _logger.LogTrace(newMessage);
                    break;
                case ILogger.LogLevel.Debug:
                    _logger.LogDebug(newMessage);
                    break;
                case ILogger.LogLevel.Info:
                    _logger.LogInfo(newMessage);
                    break;
                case ILogger.LogLevel.Warning:
                    _logger.LogWarning(newMessage);
                    break;
                case ILogger.LogLevel.Error:
                    _logger.LogError(newMessage);
                    break;
                default:
                    _logger.LogInfo(newMessage);
                    break;
            }
        }

        [DllImport("raylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetTraceLogCallback(IntPtr callback);
    }
}
