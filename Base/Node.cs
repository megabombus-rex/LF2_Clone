using Newtonsoft.Json;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone.Base
{
    // Game objects that work as a tree graph.
    public class Node
    {
#if DEBUG
        // Used for testing only.
        private Rectangle _bounds;
        private Color _boundsColor;
#endif
        // Relative to parent's transform, if root -> doesn't matter, if child of root -> _relativeTransform = _globalTransform
        public Transform _relativeTransform; 
        public Transform _globalTransform;
        
        // Ids should not repeat on one scene.
        public int _id;
        public string _name;
        public int? _parentId;

        // Parent of current Node, it is null during initiailzation or reparenting process, otherwise not.
        private Node? _parent;

        // List of current's Node children, the children are unique for a list.
        private List<Node> _children;

        // Only one component of each type is permitted, except different CustomScripts.
        private List<Component> _components;

        // Constructor used only for root node at scene creation.
        public Node()
        {
            _id = 0;
            _name = "root";
            _parent = this;
            _parentId = 0;
            _children = new List<Node>();
            _components = new List<Component>();
            _relativeTransform = new Transform()    // relative
            {
                Translation = new Vector3(),
                Rotation = new Quaternion(),
                Scale = new Vector3()
            };
            _globalTransform = new Transform()      // non-relative
            {
                Translation = new Vector3(),
                Rotation = new Quaternion(),
                Scale = new Vector3()
            };
#if DEBUG
            _bounds = new Rectangle() {
            X = _globalTransform.Translation.X,
            Y = _globalTransform.Translation.Y,
                Width = 30.0f,
                Height = 30.0f
            };
            var rand = new Random();
            _boundsColor = new Color(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256), 255);
#endif
        }

        // Constructor for new nodes added to a previous one.
        public Node(Node parent, int id, string name)
        {
            _parent = parent;
            _id = id;
            _name = name;
            _children = new List<Node>();
            _components = new List<Component>();
            parent._children.Add(this);
            _parentId = parent._id;
            _relativeTransform = new Transform()                // always relative so it's 0, 0, 0 at the beggining
            {
                Translation = new Vector3(),
                Rotation = new Quaternion(),
                Scale = new Vector3()
            };
            _globalTransform = parent._globalTransform;         // at the beggining it is the same
#if DEBUG
            _bounds = new Rectangle()
            {
                X = _globalTransform.Translation.X,
                Y = _globalTransform.Translation.Y,
                Width = 30.0f,
                Height = 30.0f
            };
            var rand = new Random();
            _boundsColor = new Color(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256), 255);
#endif
        }

        // Constructor used for proper deserialization with JSON.
        [JsonConstructor]
        public Node(int id, string name, int parentId, Transform globalTransform, Transform relativeTransform)
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
            _globalTransform = globalTransform;
            _relativeTransform = relativeTransform;
#if DEBUG
            _bounds = new Rectangle()
            {
                X = globalTransform.Translation.X,
                Y = globalTransform.Translation.Y,
                Width = 30.0f,
                Height = 30.0f
            };
            var rand = new Random();
            _boundsColor = new Color(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256), 255);
#endif
        }

        // Returns a list of nodes consisting of the children of this Node.
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
        // Sets the parent (param) node to current's one, it is not advised to use this outside initialization.
        public void SetParent(Node parent)
        {
            _parent = parent;
        }

        // When the node is created it is added to a selected node, this should enable changing the parent to a correct one.
        // The name has to be valid!
        // Removes the parent from this node, removes itself from parent's children list, sets the newParent (param) as parent.
        // Returns true if the reparenting was a success.
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

        // This method exists for freeing up the memory from nodes by letting them be destroyed.
        // Removes a node (param) from it's parent children so it won't be tracked -> GC will collect it.
        // Returns true if parent exists and the node is succesfully removed, false otherwise.
        public bool TryRemoveNode()
        {
            return _parent != null ? _parent._children.Remove(this) : false;
        }


        // This method exists for simple moving an object on scene.
        // Moves a node and all of it's children accordingly by a Vector3 vec (param).
        // Root node cannot be translated, scaled or rotated.
        public void MoveNodeByVector(Vector3 vec)
        {
            if (_id != 0)
            {
#if DEBUG
                _bounds.X += vec.X;
                _bounds.Y += vec.Y;
#endif
                _globalTransform.Translation += vec;
                _relativeTransform.Translation += vec;
            }

            foreach (var child in _children)
            {
                var currentTransGlobal = child.MoveChildren(vec);
            }
        }

        // Moves children of a node by a Vector3 vec (param) recurrently.
        private Vector3 MoveChildren(Vector3 vec)
        {
            _globalTransform.Translation += vec;
#if DEBUG
            _bounds.X += vec.X;
            _bounds.Y += vec.Y;
#endif

            foreach (var child in _children)
            {
                var currentTransGlobal = child.MoveChildren(vec);
            }
            return _globalTransform.Translation;
        }

        // This method is called on object awakening.
        public void Awake()
        {
            _bounds.X = _globalTransform.Translation.X;
            _bounds.Y = _globalTransform.Translation.Y;

            foreach (var child in _children) { 
                child.Awake();
            }
        }


        // This method is called every frame.
        public void Update()
        {
            foreach (var child in _children)
            {
                child.Update();
            }

        }

        // This method is called after updating node.
        public void Draw()
        {
            Raylib.DrawRectangleRec(_bounds, _boundsColor);
        }
    }
}
