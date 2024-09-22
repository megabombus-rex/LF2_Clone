using Raylib_cs;

namespace LF2Clone.Base
{
    // Components should work as attributes for the nodes, so each node has its own set of components
    public class Component
    {
        public Transform transform;

        public bool _isDrawable;
        public string _name;
        public int _id;

        public Component()
        {
        }

        public virtual void Draw()
        {

        }

        public virtual void Update()
        {

        }
    }
}
