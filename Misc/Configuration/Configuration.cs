namespace LF2Clone.Misc.Configuration
{
    public class Configuration
    {
        public Dictionary<string, Logging> LoggerConfigs;
        public string Version;
        public Resolution StartingScreenResolution;
        public bool Fullscreen;
        public bool BorderlessWindowed;
        public bool UseDefaultScreenResolution;
        public class Logging
        {
            public string LogLevel { get; set; } // Trace, Debug, Info, Warning, Error
        } // may add sinks later
        public class Resolution
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
