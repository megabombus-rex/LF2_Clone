using Raylib_cs;
using System.Numerics;

namespace LF2Clone.Base.Helpers
{
    public static class PositioningHelper
    {
        public static Vector2 GetCenterOfRectangle(ref Rectangle rectangle) 
        {
            return new Vector2((rectangle.Position.X + rectangle.Width) / 2, (rectangle.Position.Y + rectangle.Height) / 2);
        }
    }
}
