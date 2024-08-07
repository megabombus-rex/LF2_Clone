using LF2Clone.Base;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone.UI
{
    public class Button : Component
    {
        private Rectangle _btnBounds;
        private float _frameHeight;
        string _text;
        ButtonState _btnState;

        private readonly Texture2D _texture;
        private readonly Texture2D? _texturePressed;
        private readonly Texture2D? _textureHighlight;
        private Texture2D _currentTexture;
        private Vector3 _position;
        Callback _onclickFunc;

        public Button(string text, Texture2D texture, Texture2D? texturePressed, Texture2D? textureHighlight, Callback func, Vector3 position) : base()
        {
            _text = text;
            _texture = texture;
            _texturePressed = texturePressed;
            _textureHighlight = textureHighlight;
            _onclickFunc = func;
            _frameHeight = _texture.Height;
            _btnBounds = new Rectangle(position.X, position.Y, _texture.Width, _frameHeight);
            _position = position;
            _currentTexture = _texture;
        }

        public void Draw()
        {
            Raylib.DrawTexture(_currentTexture, (int)_position.X, (int)_position.Y, Color.White);
            Raylib.DrawText(_text, (int)_position.X, (int)_position.Y, 100, Color.White);
        }

        public void Run()
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
                _onclickFunc();
            }

            _currentTexture = _btnState switch
            {
                ButtonState.Idle => _texture,
                ButtonState.MouseHovering => _textureHighlight.HasValue ? _textureHighlight.Value : _texture,
                ButtonState.Pressed => _texturePressed.HasValue ? _texturePressed.Value : _texture,
                _ => _texture
            };

            Draw();
        }
        
        public enum ButtonState
        {
            Idle = 0,
            MouseHovering = 1,
            Pressed = 2,
            Locked = 3,
        }

        public delegate void Callback();

    }
}
