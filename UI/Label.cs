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
        private float _spacing;
        private Rectangle _backgroundRec;
        private Texture2D _backgroundTexture;
        private Font _font;

        public Label(string text, int fontSize, float spacing, int sizeX, int sizeY,Texture2D backText, Font font,
            Transform transform, bool isActive, string name, int id) : base(transform, true, isActive, name, id)
        {
            _text = text;
            _fontSize = fontSize;
            _spacing = spacing;
            _backgroundRec = new Rectangle()
            {
                Height = sizeY,
                Width = sizeX,
                Position = new Vector2(transform.Translation.X, transform.Translation.Y)
            };
            _backgroundTexture = backText;
            _font = font;
        }



        public override void Draw()
        {
            base.Draw();
            Raylib.DrawTexturePro(_backgroundTexture, _backgroundRec, _backgroundRec, _backgroundRec.Position, 0.0f, Color.Blank);
            var center = PositioningHelper.GetCenterOfRectangle(ref _backgroundRec);
            Raylib.DrawTextPro(_font, _text, center, center, _nodeGlobalTransform.Rotation.W, _fontSize, _spacing, Color.Black);
        }
    }
}
