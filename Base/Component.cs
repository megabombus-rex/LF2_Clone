using Raylib_cs;

namespace LF2Clone.Base
{
    // Components should work as attributes for the nodes, so each node has its own set of components
    public class Component
    {
        public Transform _transform;
        public bool _isDrawable;
        public bool _isActive;
                
        public Component()
        {
        }

        public virtual void Draw()
        {

        }

        public virtual void Awake()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
