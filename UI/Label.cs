using LF2Clone.Base;
using LF2Clone.Misc.Helpers;
using Raylib = Raylib_cs.Raylib;
using System.Numerics;

namespace LF2Clone.UI
{
    public class Label : UIComponent
    {
        private string _text;
        private int _fontSize;
        private float _spacing;
        private Raylib_cs.Rectangle _backgroundRec;
        private Raylib_cs.Texture2D _backgroundTexture;
        private Raylib_cs.Font _font;

        public Label(string text, int fontSize, float spacing, int sizeX, int sizeY, Raylib_cs.Texture2D backText, Raylib_cs.Font font,
             float rotation, Node? node, bool isActive, int id) : base(rotation, node, isActive, id)
        {
            _text = text;
            _fontSize = fontSize;
            _spacing = spacing;
            _backgroundRec = new Raylib_cs.Rectangle()
            {
                Height = sizeY,
                Width = sizeX,
                Position = new Vector2(node._globalTransform.Translation.X, node._globalTransform.Translation.Y)
            };
            _backgroundTexture = backText;
            _font = font;
        }

        public override void Draw()
        {
            base.Draw();
            var center = PositioningHelper.GetCenterOfRectangle(_backgroundRec);

            Raylib.DrawTexturePro(_backgroundTexture, new Raylib_cs.Rectangle(0.0f, 0.0f, new Vector2(_backgroundRec.Width, _backgroundRec.Height)), 
                _backgroundRec, center, 0.0f, Raylib_cs.Color.Blank);

            Raylib.DrawTextPro(_font, _text, new Vector2(_node._globalTransform.Translation.X, _node._globalTransform.Translation.Y), 
                center, _rotation, _fontSize, _spacing, Raylib_cs.Color.Black);
        }
    }
}
