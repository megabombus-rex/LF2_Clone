using Raylib_cs;

namespace LF2Clone.Base
{
    // Components should work as attributes for the nodes, so each node has its own set of components
    public class Component
    {
        public Transform transform;
        
        public Component()
        {
        }

        public virtual void Draw()
        {

        }
    }
}
