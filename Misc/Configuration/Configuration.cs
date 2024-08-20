namespace LF2Clone.Misc.Configuration
{
    public class Configuration
    {
        public Dictionary<string, Logging> LoggerConfigs;
        public string LoggingFilePath; // one file for all AllLog-date.log
        public string Version;
        public Resolution StartingScreenResolution;
        public bool Fullscreen;
        public bool BorderlessWindowed;
        public bool UseDefaultScreenResolution;
        public class Logging
        {
            public string LogLevel; // Trace, Debug, Info, Warning, Error
        } // may add sinks later
        public class Resolution
        {
            public int Width;
            public int Height;
        }
    }
}
