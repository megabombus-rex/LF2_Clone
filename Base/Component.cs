using LF2Clone.Base.Interfaces;
using LF2Clone.Exceptions;
using Raylib_cs;

namespace LF2Clone.Base
{
    // Components should work as attributes for the nodes, so each node has its own set of components
    public class Component : IDrawable
    {
        public bool _isDrawable;
        public bool _isActive;
        public int _id;

        protected Node _node;
                
        public Component()
        {
        }

        public Component(Node node, bool isDrawable, bool isActive, int id)
        {
            _isDrawable = isDrawable;
            _isActive = isActive;
            _id = id;
            _node = node;
        }

        public virtual void Draw()
        {
            if (!_isDrawable)
            {
                throw new NotDrawableException(string.Format("Component of type {0} is not drawable.", GetType().FullName));
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

        protected virtual void LogMessage(string message)
        {
            var fullMessage = string.Format("Node: {0} \nComponent type: {1} \nMessage: {2}", _node._name, GetType().FullName, message);
            _node.LogMessage(fullMessage);
        }
    }
}
