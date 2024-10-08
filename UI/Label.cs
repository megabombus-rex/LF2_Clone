﻿using LF2Clone.Base;
using LF2Clone.Misc.Helpers;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone.UI
{
    public class Label : UIComponent
    {
        private string _text;
        private int _fontSize;
        private float _spacing;
        private Rectangle _backgroundRec;
        private Texture2D _backgroundTexture;
        private Font _font;

        public Label(string text, int fontSize, float spacing, int sizeX, int sizeY,Texture2D backText, Font font,
             float rotation, Node node, bool isActive, int id) : base(rotation, node, isActive, id)
        {
            _text = text;
            _fontSize = fontSize;
            _spacing = spacing;
            _backgroundRec = new Rectangle()
            {
                Height = sizeY,
                Width = sizeX,
                Position = new Vector2(node._globalTransform.Translation.X, node._globalTransform.Translation.Y)
            };
            _backgroundTexture = backText;
            _font = font;
        }

        public override void Transform(Transform newTransform)
        {
            base.Transform(newTransform);
            _backgroundRec.X = newTransform.Translation.X;
            _backgroundRec.Y = newTransform.Translation.X;
        }

        public override void Draw()
        {
            base.Draw();
            var center = PositioningHelper.GetCenterOfRectangle(_backgroundRec);

            Raylib.DrawTexturePro(_backgroundTexture, new Rectangle(0.0f, 0.0f, new Vector2(_backgroundRec.Width, _backgroundRec.Height)), 
                _backgroundRec, center, 0.0f, Color.Blank);

            Raylib.DrawTextPro(_font, _text, new Vector2(_node._globalTransform.Translation.X, _node._globalTransform.Translation.Y), 
                center, _rotation, _fontSize, _spacing, Color.Black);
        }
    }
}
