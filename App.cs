using LF2Clone.UI;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone
{
    public class Application
    {
        Color _backgroundColor;
        Random _random = new();
        public void Run()
        {
            Raylib.InitWindow(1920, 1080, "Hello World");

            var buttonTex = Raylib.LoadTexture("C:\\Data\\Aseprite\\Misc\\Testing palletes\\Apple-31.png");
            var buttonTexPressed = Raylib.LoadTexture("C:\\Data\\Aseprite\\Misc\\Testing palletes\\Book-curiosity.png");
            var buttonTexHighlight = Raylib.LoadTexture("C:\\Data\\Aseprite\\Misc\\Testing palletes\\not-coconut-splendor-128.png");

            _backgroundColor = Color.White;
            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            var but = new Button("TEXT", buttonTex, buttonTexPressed, buttonTexHighlight, this.ChangeBackgroundColor, pos);
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(_backgroundColor);

                Raylib.DrawText("Hello, world!", 12, 12, 20, Color.Black);
                but.Run();
                Raylib.DrawFPS(10, 10);
                Raylib.EndDrawing();
            }

            //Raylib.DrawTextureV();

            Raylib.CloseWindow();
        }

        void ChangeBackgroundColor()
        {
            var val = _random.Next(0, 5);

            switch (val)
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