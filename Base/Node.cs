using Newtonsoft.Json;

namespace LF2Clone.Base
{
    // Game objects that work as a tree graph
    public class Node
    {
        // ids should not repeat on one scene as nodes should be destroyed on scene unload
        public int _id;
        public string _name;
        public int? _parentId;

        private Node? _parent;
        private List<Node> _children;

        // only one component of each type is permitted
        private List<Component> _components;

        // used only for root node
        public Node()
        {
            _id = 0;
            _name = "root";
            _parent = this;
            _parentId = 0;
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
            _parentId = parent._id;
        }

        // used for proper deserialization with json
        [JsonConstructor]
        public Node(int id, string name, int parentId)
        {
            if (id == 0)
            {
                _parent = this;
                _parentId = 0;
                _children = new List<Node>();
            }
            else
            {
                _parent = null;
                _parentId = parentId;
            }
            _id = id;
            _name = name;
            _children = new List<Node>();
            _components = new List<Component>();
        }

        public List<Node> GetChildren()
        {
            return _children;
        }

        // Used for setting initial children list while loading a scene.
        public void AddChild(Node node)
        {
            _children.Add(node);
        }

        // Returns current parent.
        public Node? GetParent()
        {
            return _parent;
        }

        // Used for setting initial parent while loading a scene.
        public void SetParent(Node parent)
        {
            _parent = parent;
        }

        // Removes the parent from this node, removes itself from parent's children list, sets the newParent (param) as parent.
        public bool ReparentCurrentNode(Node newParent)
        {
            if (newParent == this)
            {
                throw new InvalidOperationException("Node cannot be it's own parent, except the root node.");
            }

            if (newParent._parent != null ? newParent._parent._children.Remove(newParent) : false)
            {
                newParent._parent = this;
                _children.Add(newParent);
                return true;
            }
            return false;
        }

        // Removes a node (param) from it's parent children so it won't be tracked -> GC will collect it.
        // Returns true if parent exists and the node is succesfully removed, false otherwise.
        public bool TryRemoveNode(Node node)
        {
            return _parent != null ? _parent._children.Remove(node) : false;
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
