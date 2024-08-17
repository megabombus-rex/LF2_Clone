using LF2Clone.Base;
using LF2Clone.Systems;
using LF2Clone.Misc.Logger;
using LF2Clone.UI;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone
{
    public sealed class Application
    {
        private Color _backgroundColor;
        private ILogger? _logger;
        private string _assetsBaseRoot;
        private int _currentSceneIdIndex;
        private int[] _sceneIds;

        public Application()
        {
            _backgroundColor = Color.White;
            _assetsBaseRoot = "";
            _logger = new Logger();
        }

        // set up every system needed
        private void Setup()
        {
            _logger.LoggingLevel = ILogger.LogLevel.Info; // TODO: get it from config
            _assetsBaseRoot = string.Format("{0}\\{1}", Environment.CurrentDirectory, "\\..\\..\\..\\Assets");
            SceneManager.Instance.Setup(_logger, string.Format("{0}\\Scenes", _assetsBaseRoot));
            _currentSceneIdIndex = 0;
        }

        public void Run()
        {
            Setup();

            SceneManager.Instance.TrySetCurrentScene("default");
            // current scene = default_too

            SceneManager.Instance.ShowLoadedScenes();
            _sceneIds = SceneManager.Instance.SceneIds;


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
            SceneManager.Instance.TrySetCurrentScene(_sceneIds[_currentSceneIdIndex]);
            _currentSceneIdIndex = (_currentSceneIdIndex + 1) % _sceneIds.Length;
        }
    }
}