using LF2Clone.Base;
using LF2Clone.Base.Helpers;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone.UI
{
    public class Label : Component
    {
        private string _text;
        private int _fontSize;
        private Rectangle _backgroundRec;
        private Texture2D _backgroundTexture;

        public Label(string text, int fontSize, int sizeX, int sizeY, Texture2D backText, 
            bool isActive, string name, int id) 
            : base(true, isActive, name, id)
        {
            _text = text;
            _fontSize = fontSize;
            _backgroundRec = new Rectangle()
            {
                Height = sizeY,
                Width = sizeX,
                Position = new Vector2(0, 0)
            };
            _backgroundTexture = backText;
        }

        public override void Draw()
        {
            base.Draw();
            Raylib.DrawTexturePro(_backgroundTexture, _backgroundRec, _backgroundRec, _backgroundRec.Position, 0.0f, Color.Blank);
            var center = PositioningHelper.GetCenterOfRectangle(ref _backgroundRec);
            Raylib.DrawText(_text, (int)center.X, (int)center.Y, _fontSize, Color.Black);
        }
    }
}
