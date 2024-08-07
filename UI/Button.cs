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
        int _btnState;

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
            //_frameHeight = (float)_texture.Height / NUM_FRAMES;
            //_sourceRec = new Rectangle(0, 0, _texture.Width, _frameHeight);
            _btnBounds = new Rectangle(position.X, position.Y, _texture.Width, _frameHeight);
            //_btnBounds = new Rectangle(SCREEN_WIDTH / 2.0f - _texture.Width / 2.0f, SCREEN_HEIGHT / 2.0f - _texture.Height / NUM_FRAMES / 2.0f, (float)_texture.Width, _frameHeight);
            _position = position;
            _currentTexture = _texture;
            Console.WriteLine("BUTTON CREATED");
        }

        public void Draw()
        {
            Raylib.DrawTexture(_currentTexture, (int)_position.X, (int)_position.Y, Color.White);
        }

        public void OnClick()
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
                    _btnState = 2;
                    _currentTexture = _texturePressed.HasValue ? _texturePressed.Value : _texture;
                }
                else
                {
                    _btnState = 1;
                    _currentTexture = _textureHighlight.HasValue ? _textureHighlight.Value : _texture;
                }

                if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                {
                    _currentTexture = _texture;
                    btnAction = true;
                }
            }
            else
            {
                _btnState = 0;
                _currentTexture = _texture;
            }
            if (btnAction)
            {
                _onclickFunc();
                //Raylib.PlaySound(fxButton);

                // TODO: Any desired action
            }

            // Calculate button frame rectangle to draw depending on button state
            //_sourceRec.Y = _btnState * _frameHeight;
            //}
        }
        
        public enum ButtonState
        {
            Idle = 0,
            Pressed = 1,
            Hovering = 2,
            Locked = 3,
        }

        public delegate void Callback();

    }
}
