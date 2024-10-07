using Raylib_cs;
using System.Numerics;

namespace LF2Clone.Misc.Helpers
{
    public static class PositioningHelper
    {
        public static Vector2 GetCenterOfRectangleAbs(Rectangle rectangle)
        {
            return new Vector2(rectangle.Position.X + rectangle.Width / 2, rectangle.Position.Y + rectangle.Height / 2);
        }

        public static Vector2 GetCenterOfRectangle(Rectangle rectangle)
        {
            return new Vector2(rectangle.Width / 2, rectangle.Height / 2);
        }

        public static Quaternion Translate3DQuaternionTo2D(Quaternion rotation, RotationDirection dir)
        {
            return dir switch
            {
                RotationDirection.X => new Quaternion(rotation.X, 0.0f, 0.0f, rotation.W),
                RotationDirection.Y => new Quaternion(0.0f, rotation.Y, 0.0f, rotation.W),
                RotationDirection.Z => new Quaternion(0.0f, 0.0f, rotation.Z, rotation.W),
                _ => new Quaternion(0.0f, 0.0f, 0.0f, 1.0f)
            };
        }

        public enum RotationDirection
        {
            X,
            Y,
            Z
        }
    }
}
