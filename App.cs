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
        private Random _random;
        private ILogger? _logger;
        private string _assetsBaseRoot;
        private int _currentSceneId;

        public Application()
        {
            _backgroundColor = Color.White;
            _assetsBaseRoot = "";
            _random = new Random();
            _logger = new Logger();
        }

        private void Setup()
        {
            _assetsBaseRoot = string.Format("{0}\\{1}", Environment.CurrentDirectory, "\\..\\..\\..\\Assets");
            SceneManager.Instance.Setup(_logger, string.Format("{0}\\Scenes", _assetsBaseRoot));
        }

        public void Run()
        {
            Setup();

            var scene = new Scene(1, "default");
            var scene2 = new Scene(2, "default_too");

            SceneManager.Instance.TryLoadScene("default");
            SceneManager.Instance.TryLoadScene("default_too");
            SceneManager.Instance.ShowLoadedScenes();
            SceneManager.Instance.TrySetCurrentScene("default");
            SceneManager.Instance.TryUnloadScene("default");
            SceneManager.Instance.ShowLoadedScenes();
            SceneManager.Instance.TryLoadScene("default");
            SceneManager.Instance.ShowLoadedScenes();

            Raylib.InitWindow(960, 900, "Hello World");

            var buttonTex = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_normal.png");
            var buttonTexPressed = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_pressed.png");
            var buttonTexHighlight = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_highlight.png");

            _logger.LoggingLevel = ILogger.LogLevel.Info; // TODO: get it from config

            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            var but = new Button("TEXT", buttonTex, buttonTexPressed, buttonTexHighlight, this.ChangeBackgroundColor, pos);
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

        void ChangeScene(int sceneId)
        {
        }

        void ChangeBackgroundColor()
        {
            switch (_random.Next(0, 5))
            {
                case 0:
                    _backgroundColor = Color.White; break;
                case 1:
                    _backgroundColor = Color.Black; break;
                case 2:
                    _backgroundColor = Color.Red; break;
                case 3:
                    _backgroundColor = Color.Green; break;
                case 4:
                    _backgroundColor = Color.Blue; break;
            }
        }
    }
}