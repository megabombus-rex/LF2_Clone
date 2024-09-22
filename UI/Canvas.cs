using LF2Clone.Base;
using Raylib_cs;

namespace LF2Clone.UI
{
    public class Canvas : UIComponent
    {
        public Canvas(float rotation, Transform transform, bool isActive, string name, int id) 
            : base(rotation, transform, isActive, name, id)
        {
        }
    }
}
