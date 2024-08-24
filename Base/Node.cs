using Newtonsoft.Json;
using Raylib_cs;
using System.Numerics;

namespace LF2Clone.Base
{
    // Game objects that work as a tree graph
    public class Node
    {
#if DEBUG
        private Rectangle _bounds;
        private Color _boundsColor;
        private Vector3 _speedX;
        private float _startPositionX;
#endif
        public Transform _relativeTransform; // relative to parent's transform, if root doesn't matter, if child of root -> _transform = _globalTransform
        public Transform _globalTransform;
        // ids should not repeat on one scene as nodes should be destroyed on scene unload
        public int _id;
        public string _name;
        public int? _parentId;

        private Node? _parent;
        private List<Node> _children;

        // only one component of each type is permitted, except different CustomScripts
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
            _bounds = new Rectangle() {
            X = _globalTransform.Translation.X,
            Y = _globalTransform.Translation.Y,
            Size = new Vector2(30.0f, 30.0f)
            };
            var rand = new Random();
            _boundsColor = new Color(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256), 255);
            _speedX = new Vector3(2.0f, 0.0f, 0.0f);
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
            _relativeTransform = new Transform()                // always relative so it's 0, 0, 0 at the beggining
            {
                Translation = new Vector3(),
                Rotation = new Quaternion(),
                Scale = new Vector3()
            };
            _globalTransform = parent._globalTransform; // at the beggining it is the same
            _bounds = new Rectangle()
            {
                X = _globalTransform.Translation.X,
                Y = _globalTransform.Translation.Y,
                Size = new Vector2(30.0f, 30.0f)
            };
            var rand = new Random();
            _boundsColor = new Color(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256), 255);
            _speedX = new Vector3(2.0f, 0.0f, 0.0f);
        }

        // used for proper deserialization with json
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
            _bounds = new Rectangle()
            {
                X = _globalTransform.Translation.X,
                Y = _globalTransform.Translation.Y,
                Size = new Vector2(30.0f, 30.0f)
            };
            _globalTransform = globalTransform;
            _relativeTransform = relativeTransform;
            var rand = new Random();
            _boundsColor = new Color(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256), 255);
            _speedX = new Vector3(2.0f, 0.0f, 0.0f);
        }

        public void DrawRectangle()
        {
            
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

        public void MoveNodeByVector(Vector3 vec)
        {
#if DEBUG
            _bounds.X += vec.X;
            _bounds.Y += vec.Y;
#endif
            _globalTransform.Translation += vec;
            _relativeTransform.Translation += vec;
            Console.WriteLine(string.Format("Parent node: {0}, global position: {1}, relative position: {2}", _id, _globalTransform.Translation.ToString(), _relativeTransform.Translation.ToString()));
            foreach (var child in _children)
            {
                var currentTransGlobal = child.MoveChildren(vec);
                Console.WriteLine(string.Format("Node: {0}, global position: {1}", child._id, currentTransGlobal.ToString()));
            }
        }

        public Vector3 MoveChildren(Vector3 vec)
        {
            _globalTransform.Translation += vec;
#if DEBUG
            _bounds.X += vec.X;
            _bounds.Y += vec.Y;
#endif
            foreach (var child in _children)
            {
                var currentTransGlobal = child.MoveChildren(vec);
                Console.WriteLine(string.Format("Node: {0}, global position: {1}", child._id, currentTransGlobal.ToString()));
            }
            return _globalTransform.Translation;
        }

        public void Update()
        {
#if DEBUG
            if (Math.Abs(_globalTransform.Translation.X + _startPositionX) > 250.0f)
            {
                _speedX = -_speedX;
            }
            MoveNodeByVector(_speedX);
#endif
            Draw();
        }

        public void Draw()
        {
            //for (int i = 0; i < _components.Count; i++)
            //{
            //    if(_components[i]._isDrawable) {
            //       _components[i].Draw();
            //    }
            //}

            Raylib.DrawRectangleRec(_bounds, _boundsColor);
        }
    }
}
