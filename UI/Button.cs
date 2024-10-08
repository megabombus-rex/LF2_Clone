using LF2Clone.Base;
using LF2Clone.Misc.Helpers;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone.UI
{
    public class Button : UIComponent
    {
        // update this so rotation is added, check if highlighting works
        private Rectangle _btnBounds;
        private Rectangle _targetBounds;
        private string _text;
        private Vector2 _btnCenter;
        ButtonState _btnState;

        private readonly Texture2D _texture;
        private readonly Texture2D? _texturePressed;
        private readonly Texture2D? _textureHighlight;
        private Texture2D _currentTexture;
        private Vector2 _position;

        // may use label here or just extract it as a "Text" struct
        private Font _font;
        private float _fontSize;
        private Color _textColor;
        private float _textSpacing;

        // the textures setting may have to be moved to Awake method also, they should be cached 
        public Button(string text, Texture2D texture, Texture2D? texturePressed, Texture2D? textureHighlight,
            Font font, float fontSize, Color textColor, float textSpacing,
            float rotation, Node node, bool isActive, int id) 
            : base(rotation, node, isActive, id)
        {
            _text = text;
            _texture = texture;
            _texturePressed = texturePressed;
            _textureHighlight = textureHighlight;
            _btnBounds = new Rectangle(node._globalTransform.Translation.X, node._globalTransform.Translation.Y, _texture.Width, _texture.Height);
            _position = new Vector2(node._globalTransform.Translation.X, node._globalTransform.Translation.Y);
            _font = font;
            _fontSize = fontSize;
            _textColor = textColor;
            _textSpacing = textSpacing;
            _currentTexture = _texture;
        }

        public override void Awake()
        {
            base.Awake();
            _isActive = true;
            _isDrawable = true;
            _btnCenter = PositioningHelper.GetCenterOfRectangle(_btnBounds);
        }

        public override void Update()
        {
            var mousePoint = Raylib.GetMousePosition();
            var btnAction = false;

            // Check button state
            if (Raylib.CheckCollisionPointRec(mousePoint, _btnBounds))
            {
                if (Raylib.IsMouseButtonDown(MouseButton.Left)) _btnState = ButtonState.Pressed;
                else _btnState = ButtonState.MouseHovering;

                if (Raylib.IsMouseButtonReleased(MouseButton.Left)) btnAction = true;
            }
            else
            {
                _btnState = ButtonState.Idle;
            }
            if (btnAction)
            {
                CallbackFinished?.Invoke(this, EventArgs.Empty);
            }

            _currentTexture = _btnState switch
            {
                ButtonState.Idle => _texture,
                ButtonState.MouseHovering => _textureHighlight.HasValue ? _textureHighlight.Value : _texture,
                ButtonState.Pressed => _texturePressed.HasValue ? _texturePressed.Value : _texture,
                _ => _texture
            };
        }

        public override void Draw()
        {
            base.Draw();
            Raylib.DrawTexturePro(_currentTexture, _btnBounds, _btnBounds, Vector2.Zero, _rotation, Color.White);
            Raylib.DrawTextPro(_font, _text, _position, Vector2.Zero, _rotation, _fontSize, _textSpacing, _textColor);
        }

        public override void Destroy()
        {
            base.Destroy();

            if (CallbackFinished != null)
            {
                foreach (Callback item in CallbackFinished.GetInvocationList())
                {
                    CallbackFinished -= item;
                }
            }
        }

        public enum ButtonState
        {
            Idle = 0,
            MouseHovering = 1,
            Pressed = 2,
            Locked = 3,
        }

        public delegate void Callback(object sender, EventArgs e);

        public event Callback? CallbackFinished;
    }
}
