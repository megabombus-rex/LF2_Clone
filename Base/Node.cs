using Newtonsoft.Json;

namespace LF2Clone.Base
{
    // Game objects that work as a tree graph
    public class Node
    {
        // ids should not repeat on one scene as nodes should be destroyed on scene unload
        public int _id;
        public string _name;

        public Node _parent;
        private List<Node> _children;

        // only one component of each type is permitted
        private List<Component> _components;

        public List<Node> GetChildren()
        {
            return _children;
        }


        // used only for root node
        public Node()
        {
            _id = 0;
            _name = "root";
            _parent = this;
            _children = new List<Node>();
            _components = new List<Component>();
        }

        public Node(Node parent, int id, string name)
        {
            _parent = parent;
            _id = id;
            _name = name;
            _children = new List<Node>();
            _components = new List<Component>();
            parent._children.Add(this);
        }

        // reparenting of a node
        public bool TryAddChildNode(Node node)
        {
            if (node == this)
            {
                throw new InvalidOperationException("Node cannot be it's own parent, except the root node.");
            }

            if (node._parent._children.Remove(node))
            {
                node._parent = this;
                _children.Add(node);
                return true;
            }
            return false;
        }

        // remove a node from it's parent children so it won't be tracked -> GC will collect it
        public bool TryRemoveNode(Node node)
        {
            return _parent._children.Remove(node);
        }

        public void Update()
        {

        }

        public void Draw()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if(_components[i]._isDrawable) {
                   _components[i].Draw();
                }
            }
        }
    }
}
