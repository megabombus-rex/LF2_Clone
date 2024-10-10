using LF2Clone.Base;
using LF2Clone.Misc.Helpers;
using LF2Clone.Resources;
using System.Numerics;
using Raylib = Raylib_cs.Raylib;

namespace LF2Clone.UI
{
    public class Image : UIComponent
    {
        private Raylib_cs.Rectangle _imgBounds;
        private Vector2 _imgCenter;
        private Texture? _texture;
        private Raylib_cs.Texture2D _drawnTexture; // uses the texture but resource is loaded only once per image
        //private Raylib_cs.Image _textureImage;
        //private IntPtr _imageHandle;

        public Image(float rotation, Node? node, bool isActive, int id) 
            : base(rotation, node, isActive, id)
        {
            _imgCenter = PositioningHelper.GetCenterOfRectangle(_imgBounds);
            //_imageHandle = IntPtr.Zero;
        }

        public void SetImage(Texture texture)
        {
            _imgBounds = new Raylib_cs.Rectangle(Vector2.Zero, new Vector2(texture._texture.Width, texture._texture.Height));
            _baseRec = new Raylib_cs.Rectangle(Vector2.Zero, new Vector2(_imgBounds.Width, _imgBounds.Height));
            _texture = texture;
            _drawnTexture = _texture._texture;
            //Raylib.SetTextureWrap(_drawnTexture, Raylib_cs.TextureWrap.Clamp);
            //_textureImage = Raylib.LoadImageFromTexture(_drawnTexture);
            //_imageHandle = Marshal.AllocHGlobal(Marshal.SizeOf(_textureImage));
            //Marshal.StructureToPtr(_textureImage, _imageHandle, false);


            //Marshal.FreeHGlobal(_imageHandle);
            //unsafe
            //{
            //    _imageHandle = Marshal.AllocHGlobal(sizeof(Raylib_cs.Image));
            //}
        }

        // not implemented yet
        public void ScaleImage(Vector2 newScale)
        {
            //if (_texture == null)
            //{
            //    return;
            //}

            //if (_imageHandle == IntPtr.Zero)
            //{
            //    return;
            //}
            //UnmanagedCodeHelper.ImageResizeNN(_imageHandle, (int)newScale.X, (int)newScale.Y);
            //_drawnTexture = Raylib.LoadTextureFromImage(_textureImage);

            //LogMessage(string.Format("New scale is: {0}", newScale.ToString()));
            //_imgBounds.Width = _drawnTexture.Width;
            //_imgBounds.Height = _drawnTexture.Height;
            //_baseRec.Width = _imgBounds.Width;
            //_baseRec.Height = _imgBounds.Height;
        }

        public override void Update()
        {
            _imgBounds.Position = new Vector2(_node._globalTransform.Translation.X, _node._globalTransform.Translation.Y);
        }

        public override void Destroy()
        {
            //Marshal.DestroyStructure(_imageHandle, typeof(Raylib_cs.Image)); 
            // https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.structuretoptr?view=net-8.0&redirectedfrom=MSDN
            // not needed as the Raylib_cs.Image struct does not have a reference type inside, left as a note
            base.Destroy();
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
