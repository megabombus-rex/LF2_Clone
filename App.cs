using LF2Clone.Misc.Configuration;
using LF2Clone.Misc.Logger;
using LF2Clone.Systems;
using LF2Clone.UI;
using Newtonsoft.Json;
using Raylib = Raylib_cs.Raylib;

namespace LF2Clone
{
    public sealed class Application // only one application instance will be present
    {
        private Raylib_cs.Color _backgroundColor;
        private ILogger? _logger;
        private string _assetsBaseRoot;
        private int _currentSceneIdIndex;
        private SceneManager _sceneManager; // only one SceneManager instance will exist
        private SoundManager _soundManager; // only one SoundManager instance will exist
        private ResourceManager _resourceManager; // only one SoundManager instance will exist
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
            _backgroundColor = Raylib_cs.Color.White;
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
            if (Raylib.IsKeyPressed(Raylib_cs.KeyboardKey.Enter))
            {
                _currentResolution = 0;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }
            if (Raylib.IsKeyPressed(Raylib_cs.KeyboardKey.One))
            {
                _currentResolution = 1;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }

            if (Raylib.IsKeyPressed(Raylib_cs.KeyboardKey.Two))
            {
                _currentResolution = 2;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }

            if (Raylib.IsKeyPressed(Raylib_cs.KeyboardKey.Three))
            {
                _currentResolution = 3;
                _screenWidth = _resolutions[_currentResolution].width;
                _screenHeight = _resolutions[_currentResolution].height;
            }

            if (Raylib.IsKeyPressed(Raylib_cs.KeyboardKey.Four))
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
        public void Setup()
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
            var RMlogger = new Logger<ResourceManager>(logPath, _configuration.LoggerConfigs.ContainsKey("ResourceManager") ? _configuration.LoggerConfigs["ResourceManager"].LogLevel : defaultLoggingLevel);
            var SMlogger = new Logger<SceneManager>(logPath, _configuration.LoggerConfigs.ContainsKey("Application") ? _configuration.LoggerConfigs["Application"].LogLevel : defaultLoggingLevel);
            var SoMlogger = new Logger<SoundManager>(logPath, _configuration.LoggerConfigs.ContainsKey("SoundManager") ? _configuration.LoggerConfigs["SoundManager"].LogLevel : defaultLoggingLevel);

            var logRaylib = new Logger<RaylibLoggerWrapper>(logPath, _configuration.LoggerConfigs.ContainsKey("BaseLog") ? _configuration.LoggerConfigs["BaseLog"].LogLevel : defaultLoggingLevel);
            RaylibLoggerWrapper wrapper = new RaylibLoggerWrapper(logRaylib);
            wrapper.Initialize(); // experimental
            _logger.LogInfo("Loggers initialized.");
            
            DeleteOldLogFiles();

            // initialize systems
            _resourceManager = new ResourceManager(RMlogger);
            _soundManager = new SoundManager(SoMlogger, _resourceManager);
            _sceneManager = new SceneManager(SMlogger, _resourceManager, _soundManager);
            _logger.LogInfo("Systems initialized.");


            // setup systems
            _assetsBaseRoot = string.Format("{0}\\{1}", Environment.CurrentDirectory, "..\\..\\..\\Assets"); // it needs to be changed
            _resourceManager.Setup();
            _sceneManager.Setup(string.Format("{0}\\Scenes", _assetsBaseRoot));
            _soundManager.Setup();
            _logger.LogInfo("Systems setup finished.");
            // Audio device is ok, have to add it so every SFX is played if needed

            Raylib.InitAudioDevice();

            // setup application
            _currentSceneIdIndex = 0;
            _currentResolution = 0;
            _logger.LogDebug("App setup finished.");
        }

        public void Run()
        {
            Setup();

            InitWindow();

            _resourceManager.Awake();
            _soundManager.Awake();
            _sceneManager.Awake();

            _sceneManager.TrySetCurrentSceneAsync("default").Wait();
            _sceneManager.ShowLoadedScenes();

            var btnImgPath = Path.GetFullPath(string.Format("{0}\\{1}", _assetsBaseRoot, "UI\\Buttons\\Button_normal.png"));

            _resourceManager.LoadResource(btnImgPath);

            var image = _resourceManager._loadedTexturesDict[btnImgPath];

            var node = _sceneManager.CurrentScene.GetNodeById(1);

            node.MessageSent += _logger.LogFromExternal;

            var imgComponent = new Image(image._texture.Width, image._texture.Height, 0.0f, node, true, 1);
            node.AddComponent(imgComponent);

            Raylib.SetTargetFPS(60);

            // game loop in here
            while (!Raylib.WindowShouldClose())
            {
                ChangeResolution(_screenWidth, _screenHeight);
                Raylib.BeginDrawing();
                Raylib.ClearBackground(_backgroundColor);

                _resourceManager.Update();
                _soundManager.Update();
                _sceneManager.Update(); // here all of the logic is made

                Raylib.DrawFPS(10, 10);
                Raylib.EndDrawing();
            }

            _sceneManager.Destroy();
            _soundManager.Destroy();
            _resourceManager.Destroy();
            _logger.Dispose();

            Raylib.CloseWindow();
        }

        private void ChangeScene(object sender, EventArgs e)
        {
            _sceneManager.TrySetCurrentSceneAsync(_sceneManager.SceneIds[_currentSceneIdIndex]).Wait(); // possible to do async, events not work well with async
            _sceneManager.AwakeCurrentScene();
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
                Console.WriteLine(ex.ToString());
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

            var files = Directory.GetFiles(_configuration.LoggingFilePath).Where(x => !x.EndsWith(".gitkeep")).ToArray();

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