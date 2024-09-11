using LF2Clone.Base;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone.UI
{
    public class Button : Component
    {
        private Rectangle _btnBounds;
        private string _text;
        ButtonState _btnState;

        private readonly Texture2D _texture;
        private readonly Texture2D? _texturePressed;
        private readonly Texture2D? _textureHighlight;
        private Texture2D _currentTexture;
        private Vector2 _position;

        // the textures setting may have to be moved to Awake method also, they should be cached 
        public Button(string text, Texture2D texture, Texture2D? texturePressed, Texture2D? textureHighlight, 
            Vector2 position, bool isActive, string name, int id) 
            : base(true, isActive, name, id)
        {
            _text = text;
            _texture = texture;
            _texturePressed = texturePressed;
            _textureHighlight = textureHighlight;
            _btnBounds = new Rectangle(position.X, position.Y, _texture.Width, _texture.Height);
            _position = position;
            _currentTexture = _texture;
        }

        public override void Awake()
        {
            base.Awake();
            _isActive = true;
            _isDrawable = true;
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
            Raylib.DrawTexture(_currentTexture, (int)_position.X, (int)_position.Y, Color.White);
            Raylib.DrawText(_text, (int)_position.X, (int)_position.Y, 100, Color.White);
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
