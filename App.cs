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
        private int _screenWidth;
        private int _screenHeight;
        private Dictionary<int, (int width, int height)> _resolutions;
        private int _currentResolution;

        public Application()
        {
            _backgroundColor = Color.White;
            _assetsBaseRoot = "";
            _resolutions = new();
        }

        private void AddResolutions()
        {
            _resolutions.Add(1, (1920, 1080));
            _resolutions.Add(2, (960, 540));
            _resolutions.Add(3, (480, 270));
            _resolutions.Add(4, (240, 135));
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
            _screenHeight = _configuration.StartingScreenResolution.Height;
            _screenWidth = _configuration.StartingScreenResolution.Width;

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
            _currentResolution = 1;
            AddResolutions();

            _logger.LogDebug("App setup finished.");
        }

        public async Task RunAsync()
        {
            await SetupAsync();
            _sceneManager.TrySetCurrentScene("default");
            // current scene = default_too

            _sceneManager.ShowLoadedScenes();
            _sceneIds = _sceneManager.SceneIds;


            Raylib.InitWindow(_screenWidth, _screenHeight, "Hello World");

            var buttonTex = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_normal.png");
            var buttonTexPressed = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_pressed.png");
            var buttonTexHighlight = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_highlight.png");

            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            var but = new Button("TEXT", buttonTex, buttonTexPressed, buttonTexHighlight, ChangeScene, pos);
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsWindowResized() && !Raylib.IsWindowFullscreen())
                {
                    _screenWidth = Raylib.GetScreenWidth();
                    _screenHeight = Raylib.GetScreenHeight();
                }

                // TODO: change placement of UI in currently loaded scene -> resize -> sceneManager.resizeCurrentScene(resolution)
                if (Raylib.IsKeyPressed(KeyboardKey.One)) _currentResolution = 1;
                if (Raylib.IsKeyPressed(KeyboardKey.Two)) _currentResolution = 2;
                if (Raylib.IsKeyPressed(KeyboardKey.Three)) _currentResolution = 3;
                if (Raylib.IsKeyPressed(KeyboardKey.Four)) _currentResolution = 4;
                Raylib.SetWindowSize(_resolutions[_currentResolution].width, _resolutions[_currentResolution].height);


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