using LF2Clone.Base;
using LF2Clone.Misc.Helpers;
using LF2Clone.Resources;
using Raylib = Raylib_cs.Raylib;
using System.Numerics;

namespace LF2Clone.UI
{
    public class Image : UIComponent
    {
        private Raylib_cs.Rectangle _imgBounds;
        private Vector2 _imgCenter;
        private Texture? _texture;
        private Raylib_cs.Texture2D _drawnTexture; // uses the texture but resource is loaded only once per image
        private Raylib_cs.Image _textureImage;

        public Image(float sizeX, float sizeY, float rotation, Node node, bool isActive, int id) 
            : base(rotation, node, isActive, id)
        {
            _baseRec = new Raylib_cs.Rectangle(Vector2.Zero, new Vector2(_imgBounds.Width, _imgBounds.Height));
            _imgBounds = new Raylib_cs.Rectangle(new Vector2(node._globalTransform.Translation.X, node._globalTransform.Translation.Y), new Vector2(sizeX, sizeY));
            _imgCenter = PositioningHelper.GetCenterOfRectangle(_imgBounds);
        }

        public void SetImage(Texture texture)
        {
            _texture = texture;
            _drawnTexture = _texture._texture;
            _textureImage = Raylib.LoadImageFromTexture(_drawnTexture);
            LogMessage("Image set.");
        }

        public void ScaleImage(Vector2 newScale)
        {
            if (_texture == null)
            {
                return;
            }

            unsafe
            {
                var imageFromText = _textureImage;
                Raylib.ImageResizeNN(&imageFromText, (int)newScale.X, (int)newScale.Y);
                _drawnTexture = Raylib.LoadTextureFromImage(_textureImage);
            }
            _imgBounds.Width = _drawnTexture.Width;
            _imgBounds.Height = _drawnTexture.Height;
        }

        public override void Update()
        {
            _imgBounds.Position = new Vector2(_node._globalTransform.Translation.X, _node._globalTransform.Translation.Y);
        }

        public override void Draw()
        {
            Raylib.DrawRectanglePro(_imgBounds, Vector2.Zero, 0.0f, Raylib_cs.Color.Blank);
            if (_texture == null)
            {
                return;
            }
            Raylib.DrawTexturePro(_drawnTexture, _baseRec, _imgBounds, Vector2.Zero, 0.0f, Raylib_cs.Color.White);
            //Raylib.DrawTextureEx(_drawnTexture, _imgBounds.Position, 0.0f, _nodeGlobalTransform.Scale.X, Color.White);
        }
    }
}
