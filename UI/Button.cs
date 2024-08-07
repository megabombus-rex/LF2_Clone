using LF2Clone.Base;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone.UI
{
    public class Button : Component
    {
        private const int SCREEN_WIDTH = 1920;
        private const int SCREEN_HEIGHT = 1080;
        //private const int NUM_FRAMES = 3;

        private Rectangle _btnBounds;
        private Rectangle _sourceRec;
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
            Console.WriteLine("BUTTON CREATED");
        }

        public void Draw()
        {
            Raylib.DrawTexture(_currentTexture, (int)_position.X, (int)_position.Y, Color.White);
            Raylib.DrawText(_text, (int)_position.X, (int)_position.Y, 10, Color.White);
        }

        public void Run()
        {
            //if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            //{
            var mousePoint = Raylib.GetMousePosition();
            var btnAction = false;

            // Check button state
            if (Raylib.CheckCollisionPointRec(mousePoint, _btnBounds))
            {
                //Console.WriteLine("Mouse inside button");
                if (Raylib.IsMouseButtonDown(MouseButton.Left))
                {
                    _btnState = ButtonState.Pressed;
                    _currentTexture = _texturePressed.HasValue ? _texturePressed.Value : _texture;
                }
                else
                {
                    _btnState = ButtonState.MouseHovering;
                    _currentTexture = _textureHighlight.HasValue ? _textureHighlight.Value : _texture;
                }

                if (Raylib.IsMouseButtonReleased(MouseButton.Left)) btnAction = true;
            }
            else
            {
                _btnState = ButtonState.Idle;
                _currentTexture = _texture;
            }
            if (btnAction)
            {
                _onclickFunc();
                //Raylib.PlaySound(fxButton);
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
