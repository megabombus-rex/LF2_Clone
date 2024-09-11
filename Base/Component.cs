using LF2Clone.Base.Interfaces;

namespace LF2Clone.Base
{
    // Components should work as attributes for the nodes, so each node has its own set of components
    public class Component : IDrawable
    {
        public bool _isDrawable;
        public bool _isActive;
        public string _name;
        public int _id;
                
        public Component()
        {
            _name = "comp";
        }

        public Component(bool isDrawable, bool isActive, string name, int id)
        {
            _isDrawable = isDrawable;
            _isActive = isActive;
            _name = name;
            _id = id;
        }

        public virtual void Draw()
        {
            if (!_isDrawable)
            {
                throw new InvalidOperationException(string.Format("Component {0} is not drawable.", _name));
            }  
        }

        public virtual void Awake()
        {
            _isActive = true;
        }

        public virtual void Activate()
        {
            _isActive = true;
        }

        public virtual void Update()
        {

        }

        public virtual void Deactivate()
        {
            _isActive = false;
        }

        public virtual void Destroy()
        {
            
        }
    }
}
