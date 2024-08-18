using LF2Clone.Misc.Configuration;
using LF2Clone.Misc.Logger;
using LF2Clone.Systems;
using LF2Clone.UI;
using Newtonsoft.Json;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone
{
    public sealed class Application // only one application instance will be present
    {
        private Color _backgroundColor;
        private ILogger<Application>? _logger;
        private string _assetsBaseRoot;
        private int _currentSceneIdIndex;
        private int[] _sceneIds;
        private SceneManager _sceneManager; // only one SceneManager instance will exist
        private Configuration _configuration;
        private string _appVersion;

        public Application()
        {
            _backgroundColor = Color.White;
            _assetsBaseRoot = "";
        }

        // set up every system needed
        private async Task SetupAsync()
        {
            if (!ReadConfig())
            {
                Environment.Exit(0);
                return;
            }

            // set current version
            _appVersion = _configuration.Version;

            // initialize systems
            _sceneManager = new SceneManager();

            // initialize loggers
            _logger = new Logger<Application>();
            var SMlogger = new Logger<SceneManager>();

            // set logging levels, 
            var defaultLoggingLevel = "Info";
            _logger.ParseAndSetLoggingLevel(_configuration.LoggerConfigs.ContainsKey("Application") ? _configuration.LoggerConfigs["Application"].LogLevel : defaultLoggingLevel);
            SMlogger.ParseAndSetLoggingLevel(_configuration.LoggerConfigs.ContainsKey("SceneManager") ? _configuration.LoggerConfigs["SceneManager"].LogLevel : defaultLoggingLevel);

            // setup systems
            _assetsBaseRoot = string.Format("{0}\\{1}", Environment.CurrentDirectory, "\\..\\..\\..\\Assets");
            await _sceneManager.SetupAsync(SMlogger, string.Format("{0}\\Scenes", _assetsBaseRoot));

            // setup application
            _currentSceneIdIndex = 0;
            _logger.LogDebug("App setup finished.");
        }

        public async Task RunAsync()
        {
            await SetupAsync();
            _sceneManager.TrySetCurrentScene("default");
            // current scene = default_too

            _sceneManager.ShowLoadedScenes();
            _sceneIds = _sceneManager.SceneIds;


            Raylib.InitWindow(960, 900, "Hello World");

            var buttonTex = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_normal.png");
            var buttonTexPressed = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_pressed.png");
            var buttonTexHighlight = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_highlight.png");

            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            var but = new Button("TEXT", buttonTex, buttonTexPressed, buttonTexHighlight, ChangeScene, pos);
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(_backgroundColor);
                but.Run();
                Raylib.DrawFPS(10, 10);
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        void ChangeScene()
        {
            _sceneManager.TrySetCurrentScene(_sceneIds[_currentSceneIdIndex]);
            _currentSceneIdIndex = (_currentSceneIdIndex + 1) % _sceneIds.Length;
        }

        bool ReadConfig() // serial not async
        {
            try
            {
                using (var sw = new StreamReader(string.Format("{0}\\{1}", Environment.CurrentDirectory, "\\..\\..\\..\\appsettings.json")))
                {
                    var config = sw.ReadToEnd();
                    _configuration = JsonConvert.DeserializeObject<Configuration>(config);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }
}