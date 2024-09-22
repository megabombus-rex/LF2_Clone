using LF2Clone.Components;
using LF2Clone.Misc.Configuration;
using LF2Clone.Misc.Logger;
using LF2Clone.Resources;
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
        private ILogger? _logger;
        private string _assetsBaseRoot;
        private int _currentSceneIdIndex;
        private SceneManager _sceneManager; // only one SceneManager instance will exist
        private SoundManager _soundManager; // only one SoundManager instance will exist
        private Configuration _configuration;
        private string _appVersion;
        private int _screenWidth;
        private int _screenHeight;
        private Dictionary<int, (int width, int height)> _resolutions;
        private int _currentResolution;
        private int _currentMonitor;
        private bool _isBorderless;
        private bool _isFullscreen;

        public Application()
        {
            _backgroundColor = Color.White;
            _assetsBaseRoot = "";
            _resolutions = new();
        }
        private void InitWindow()
        {
            Raylib.InitWindow(_screenWidth, _screenHeight, "Hello World");
            _currentMonitor = Raylib.GetCurrentMonitor();

            if (_configuration.BorderlessWindowed)
            {
                Raylib.ToggleBorderlessWindowed();
                _isBorderless = true;
            }

            if (_configuration.Fullscreen)
            {
                Raylib.ToggleFullscreen();
                _isFullscreen = true;
            }
            
            if (!_configuration.Fullscreen && !_configuration.BorderlessWindowed)
            {
                SetWindowPositionCentered(_currentMonitor, _screenWidth, _screenHeight);
            }

            AddResolutions();
        }
        private void AddResolutions()
        {
            _resolutions.Add(0, (_screenWidth, _screenHeight));
            _resolutions.Add(1, (1920, 1080));
            _resolutions.Add(2, (960, 540));
            _resolutions.Add(3, (480, 270));
            _resolutions.Add(4, (240, 135));
        }
        private void ChangeResolution(int currentWidth, int currentHeight)
        {
            // TODO: change placement of UI in currently loaded scene -> resize -> sceneManager.resizeCurrentScene(resolution)
            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                _currentResolution = 0;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.One))
            {
                _currentResolution = 1;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Two))
            {
                _currentResolution = 2;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Three))
            {
                _currentResolution = 3;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Four))
            {
                _currentResolution = 4;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }

            if ((currentHeight != _screenHeight || currentWidth != _screenWidth) && !(_isFullscreen || _isBorderless))
            {
                SetWindowPositionCentered(_currentMonitor, _screenWidth, _screenHeight);
                Raylib.SetWindowSize(_screenWidth, _screenHeight);
            }
        }
        private void SetWindowPositionCentered(int currentMonitor, int width, int height)
        {
            var setX = width == 0 ? 0 : (Raylib.GetMonitorWidth(currentMonitor) / 2) - (width / 2);
            var setY = height == 0 ? 0 : (Raylib.GetMonitorHeight(currentMonitor) / 2) - (height / 2);

            _logger.LogInfo(string.Format("SetX: {0} SetY: {1}", setX, setY));

            Raylib.SetWindowPosition(setX, setY);
            _logger.LogInfo(string.Format("Width set: {0} Height set: {1}. Position: {2}", width.ToString(), height.ToString(), Raylib.GetWindowPosition().ToString()));
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
            if (!_configuration.UseDefaultScreenResolution)
            {
                _screenHeight = _configuration.StartingScreenResolution.Height;
                _screenWidth = _configuration.StartingScreenResolution.Width;
            }
            else
            {
                _screenHeight = 0;
                _screenWidth = 0;
            }

            // initialize loggers
            var logPath = string.IsNullOrEmpty(_configuration.LoggingFilePath) ? "" : _configuration.LoggingFilePath;
            var defaultLoggingLevel = "Info";

            _logger = new Logger<Application>(logPath, _configuration.LoggerConfigs.ContainsKey("Application") ? _configuration.LoggerConfigs["Application"].LogLevel : defaultLoggingLevel);
            var SMlogger = new Logger<SceneManager>(logPath, _configuration.LoggerConfigs.ContainsKey("Application") ? _configuration.LoggerConfigs["Application"].LogLevel : defaultLoggingLevel);
            var SoMlogger = new Logger<SoundManager>(logPath, _configuration.LoggerConfigs.ContainsKey("SoundManager") ? _configuration.LoggerConfigs["SoundManager"].LogLevel : defaultLoggingLevel);

            DeleteOldLogFiles();

            // initialize systems
            _sceneManager = new SceneManager(SMlogger);
            _soundManager = new SoundManager(SoMlogger);

            // setup systems
            _assetsBaseRoot = string.Format("{0}\\{1}", Environment.CurrentDirectory, "..\\..\\..\\Assets");
            await _sceneManager.SetupAsync(string.Format("{0}\\Scenes", _assetsBaseRoot));

            // Audio device is ok, have to add it so every SFX is played if needed

            Raylib.InitAudioDevice();

            var music = Raylib.LoadMusicStream(_assetsBaseRoot + "\\Sounds\\kerosene x pluh.mp3");
            var resId = Guid.NewGuid();

            var res = new SFX()
            {
                _id = resId,
                _path = _assetsBaseRoot + "\\Sounds\\kerosene x pluh.mp3",
                _name = "kerosene",
                _type = SFX.SoundType.Music,
                _durationInSeconds = music.Stream.SampleRate * music.Stream.SampleSize,
                _value = music
            };
            _soundManager.AddSFX(resId, res);
            var sfx = new SFXSoundPlayer(5, "SFX1", resId);

            sfx.Play += _soundManager.Play;

            sfx.PlayCurrentSound(); // also stop playing event should be created

            // setup application
            _currentSceneIdIndex = 0;
            _currentResolution = 0;
            _logger.LogDebug("App setup finished.");
        }

        public async Task RunAsync()
        {
            await SetupAsync();
            _sceneManager.TrySetCurrentScene("default");
            _sceneManager.ShowLoadedScenes();

            InitWindow();
            var buttonTex = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_normal.png");
            var buttonTexPressed = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_pressed.png");
            var buttonTexHighlight = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_highlight.png");


            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            var but = new Button("TEXT", buttonTex, buttonTexPressed, buttonTexHighlight, ChangeScene, pos);
            Raylib.SetTargetFPS(60);

            _sceneManager.CurrentScene.Awake();

            // game loop in here
            while (!Raylib.WindowShouldClose())
            {
                ChangeResolution(_screenWidth, _screenHeight);
                Raylib.BeginDrawing();
                Raylib.ClearBackground(_backgroundColor);
                but.Run();
                _sceneManager.CurrentScene.Update();
                
                Raylib.DrawFPS(10, 10);
                Raylib.EndDrawing();
            }

            _logger.Dispose();
            _sceneManager.Destroy();

            Raylib.CloseWindow();
        }

        void ChangeScene()
        {
            _sceneManager.TrySetCurrentScene(_sceneManager.SceneIds[_currentSceneIdIndex]);
            _sceneManager.CurrentScene.Awake();
            _currentSceneIdIndex = (_currentSceneIdIndex + 1) % _sceneManager.SceneIds.Length;
        }

        bool ReadConfig()
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

        void DeleteOldLogFiles()
        {
            if (_configuration.LoggingDaysOfLife < 0)
            {
                _logger.LogInfo("LoggingDaysOfLife configuration parameter is incorrect. \n No logs will be deleted.");
                return;
            }

            var files = Directory.GetFiles(_configuration.LoggingFilePath).Where(x => !x.StartsWith(".git")).ToArray();

            if (files.Length < 1)
            {
                _logger.LogInfo("No logs found.");
                return;
            }

            var currDate = DateTime.UtcNow.Date;

            foreach (var file in files)
            {
                try
                {
                    if(File.GetCreationTime(file).Date.AddDays(_configuration.LoggingDaysOfLife) < currDate.Date)
                    {
                        File.Delete(file);
                    }

                }
                catch
                {
                    _logger.LogError(string.Format("Error while checking file {0}. \n No further logs will be removed.", file));
                    return;
                }
            }
        }
    }
}