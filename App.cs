using LF2Clone.UI;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone
{
    public class Application
    {
        Color _backgroundColor;
        Random _random;

        private string _assetsBaseRoot;

        public Application()
        {
            _assetsBaseRoot = string.Format("{0}\\{1}", Environment.CurrentDirectory, "\\..\\..\\..\\Assets");
            _backgroundColor = Color.White;
            _random = new Random();
        }

        public void Run()
        {
            Raylib.InitWindow(960, 900, "Hello World");
            var buttonTex = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_normal.png");
            var buttonTexPressed = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_pressed.png");
            var buttonTexHighlight = Raylib.LoadTexture(_assetsBaseRoot + "\\UI\\Buttons\\Button_highlight.png");

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