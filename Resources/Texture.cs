namespace LF2Clone.Resources
{
    public class Texture : Resource
    {
        public Raylib_cs.Texture2D _texture;
        public float _sizeX;
        public float _sizeY;

        public void ChangeTexture(Raylib_cs.Texture2D texture)
        {
            _texture = texture;
        }
    }
}
