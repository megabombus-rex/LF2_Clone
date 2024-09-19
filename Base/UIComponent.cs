using Raylib_cs;

namespace LF2Clone.Base
{
    public class UIComponent : Component
    {
        public UIComponent(float rotation, Transform transform, bool isActive, string name, int id) : base(transform, true, isActive, name, id)
        {
            _rotation = rotation;
        }

        public void SetRotation(float newRotation)
        {
            _rotation = newRotation;
        }

        public const float RADIANS_TO_DEGREES = (float)(180.0 / Math.PI);

        protected float _rotation;
    }
}
