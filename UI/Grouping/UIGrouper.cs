using LF2Clone.Base;
using Raylib_cs;

namespace LF2Clone.UI.Grouping
{
    public class UIGrouper : UIComponent
    {
        public UIGrouper(float rotation, Transform transform, bool isActive, string name, int id) : 
            base(rotation, transform, isActive, name, id)
        {
        }
    }
}