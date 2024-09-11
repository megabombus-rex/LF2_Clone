using LF2Clone.Base.Interfaces;
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
        private float _baseWidth;
        private float _baseHeight;
        private Color _boundsColor;
#endif
        // Relative to parent's transform, if root -> doesn't matter, if child of root -> _relativeTransform = _globalTransform
        public Transform _relativeTransform; 
        public Transform _globalTransform;
        private float _rotation;

        // Ids should not repeat on one scene.
        public int _id;
        public string _name;
        public int? _parentId;

        // Parent of current Node, it is null during initiailzation or reparenting process, otherwise not.
        private Node? _parent;

        // List of current's Node children, the children are unique for a list.
        private List<Node> _children;

        // Only one component of each type is permitted, except different CustomScripts (different derived classes are needed though).
        private List<Component> _components = new();
        private IEnumerable<Component> _activeComponents = Enumerable.Empty<Component>();
        private IEnumerable<IDrawable> _drawableComponents = Enumerable.Empty<IDrawable>();

        #region Constructors

        /// <summary>
        /// Constructor used only for root node at scene creation.
        /// </summary>
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

        /// <summary>
        /// Constructor for new nodes added to a previous one.
        /// </summary>
        /// <param name="parent"> Parent node. </param>
        /// <param name="id"> Node id. </param>
        /// <param name="name"> Node name. </param>
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

        /// <summary>
        /// Constructor used for proper deserialization with JSON.
        /// </summary>
        /// <param name="id"> Node id. </param>
        /// <param name="name"> Node name. </param>
        /// <param name="parentId"> Parent node id. </param>
        /// <param name="globalTransform"> Deserialized global transform. </param>
        /// <param name="relativeTransform"> Deserialized relative transform. </param>
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

        #endregion

        #region Children-parent relations

        /// <summary>
        /// Returns a list of nodes consisting of the children of this Node.
        /// </summary>
        /// <returns></returns>
        public List<Node> GetChildren()
        {
            return _children;
        }

        /// <summary>
        /// Used for setting initial children list while loading a scene.
        /// </summary>
        /// <param name="node"> Child node. </param>
        public void AddChild(Node node)
        {
            _children.Add(node);
        }

        /// <summary>
        /// Returns current parent.
        /// </summary>
        /// <returns></returns>
        public Node? GetParent()
        {
            return _parent;
        }

        /// <summary>
        /// Used for setting initial parent while loading a scene.
        /// Sets the parent node to current's one, it is not advised to use this outside initialization.
        /// </summary>
        /// <param name="parent"> Parent node. </param>
        public void SetParent(Node parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// When the node is created it is added to a selected node, this should enable changing the parent to a correct one.
        /// The name has to be valid!
        /// Removes the parent from this node, removes itself from parent's children list, sets the newParent (param) as parent.
        /// Returns true if the reparenting was a success.
        /// </summary>
        /// <param name="newParent"> New parent of current node. </param>
        /// <exception cref="ArgumentException"> Throws when the node is its parent. </exception>
        /// <exception cref="InvalidOperationException"> Throws when there were problems while removing the node. </exception>
        public void ReparentCurrentNode(Node newParent)
        {
            if (newParent == this)
            {
                throw new ArgumentException("Node cannot be it's own parent, except the root node.");
            }

            if (TryRemoveNodeFromParent())
            {
                newParent._parent = this;
                _children.Add(newParent);
                return;
            }
            throw new InvalidOperationException("Node could not be removed from it's previous parent.");
        }

        /// <summary>
        /// This method exists for freeing up the memory from nodes by letting them be destroyed.
        /// Removes a node (param) from it's parent children so it won't be tracked -> GC will collect it.
        /// Returns true if parent exists and the node is succesfully removed, false otherwise.
        /// </summary>
        /// <returns> True if node was removed, false otherwise or if the parent is null. </returns>
        public bool TryRemoveNodeFromParent()
        {
            return _parent != null ? _parent._children.Remove(this) : false;
        }

        #endregion

        #region Components

        // there will be issues with serialization, todo
        /// <summary>
        /// Add a component to current Node.
        /// </summary>
        /// <param name="component"> Component to be added. </param>
        /// <exception cref="InvalidOperationException"> Thrown when a component of given type exists. </exception>
        public void AddComponent(Component component)
        {
            var type = component.GetType();

            if (_components.Count > 0 && _components.Any(x => x.GetType().FullName == type.FullName))
            {
                throw new InvalidOperationException(string.Format("Commponent with the same type ({0}) already exists. Component name: {1}", type.ToString(), component._name));
            }

            if (_components.Count > 0 && _components.Any(x => x._id == component._id))
            {
                component._id = Helpers.NamingHelper.GetNextAvailableId(_components.Select(x => x._id));
            }

            if (_components.Count > 0 && _components.Any(x => x._name == component._name))
            {
                component._name = Helpers.NamingHelper.GetNextValidName(_components.Select(x => x._name), component._name);
            }

            _components.Add(component);

            if (!component._isActive)
            {
                component.Awake();
            }
            _activeComponents = _components.Where(x => x._isActive);

            if (component._isDrawable)
            {
                _drawableComponents = _activeComponents.Where(x => x._isDrawable);
            }
        }

        /// <summary>
        /// Removes a component from current Node.
        /// </summary>
        /// <param name="component"> Component to be removed. </param>
        /// <exception cref="InvalidOperationException"> Thrown when the component cannot be removed. </exception>
        public void RemoveComponent(Component component)
        {
            if (!_components.Remove(component))
            {
                throw new InvalidOperationException(string.Format("Could not remove component {0}.", component._name));
            }

            if (component._isActive)
            {
                _activeComponents = _components.Where(x => x._isActive);
            }

            if (component._isDrawable)
            {
                _drawableComponents = _activeComponents.Where(x => x._isDrawable);
            }
            component.Destroy();
        }

        /// <summary>
        /// Activate component by its id.
        /// </summary>
        /// <param name="id"> Id of a given component. </param>
        /// <exception cref="InvalidOperationException"> Thrown when the component is already active.</exception>
        /// <exception cref="KeyNotFoundException"> Thrown when the component was not found. </exception>
        public void ActivateComponent(int id)
        {
            if (_activeComponents.Any(x => x._id == id))
            {
                throw new InvalidOperationException(string.Format("Component with id {0} does is already active!", id));
            }

            var component = _components.FirstOrDefault();

            if (component == null)
            {
                throw new KeyNotFoundException(string.Format("Component with id {0} not found.", id));
            }

            component.Activate();
            _activeComponents = _components.Where(x => x._isActive);
            _drawableComponents = _activeComponents.Where(x => x._isDrawable);
        }

        /// <summary>
        /// Deactivate a component with id.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="KeyNotFoundException"> Thrown when the component was not found. </exception>
        public void DeactivateComponent(int id)
        {
            var component = _activeComponents.FirstOrDefault(x => x._id == id);

            if (component == null)
            {
                throw new KeyNotFoundException(string.Format("Component with id {0} not found.", id));
            }

            component.Deactivate();
            _activeComponents = _components.Where(x => x._isActive);

            if (component._isDrawable)
            {
                _drawableComponents = _activeComponents.Where(x => x._isDrawable);
            }
        }

        /// <summary>
        /// Gets component by id.
        /// </summary>
        /// <param name="id"> Component id. </param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"> Thrown when the component was not found. </exception>
        public Component GetComponentById(int id)
        {
            return _components.FirstOrDefault(x => x._id == id) ?? throw new KeyNotFoundException(string.Format("Component with id {0} not found.", id));
        }

        public List<Component> GetAllComponents() { return _components; }

        #endregion

        #region Node transformations

        /// <summary>
        /// Rotates a node and its children by a quaternion.
        /// </summary>
        /// <param name="quaternion"> Quaternion by which the node is rotated.</param>
        /// <returns></returns>
        public Quaternion RotateNode(Quaternion quaternion)
        {
            if (_id != 0)
            {
                _rotation += quaternion.W;
                _globalTransform.Rotation *= quaternion;
                _relativeTransform.Rotation *= quaternion;
            }

            foreach (var child in _children)
            {
                var currentGlobalRotation = child.RotateChildren(quaternion);
            }

            return _globalTransform.Rotation;
        }

        /// <summary>
        /// Rotates children of a node by a quaternion.
        /// </summary>
        /// <param name="quaternion"> Quaternion by which the children are rotated.</param>
        /// <returns></returns>
        private Quaternion RotateChildren(Quaternion quaternion)
        {
            _globalTransform.Rotation *= quaternion;
            _rotation += quaternion.W;

            foreach (var child in _children)
            {
                var currentGlobalRotation = child.RotateChildren(quaternion);
            }

            return _globalTransform.Rotation;
        }


        /// <summary>
        /// This method exists for simple moving an object on scene.
        /// Moves a node and all of it's children accordingly by a Vector3 vec (param).
        /// Root node cannot be translated, scaled or rotated.
        /// </summary>
        /// <param name="vec"> Vector3 by which the node (and its children) are moved.</param>
        /// <returns></returns>
        public Vector3 MoveNodeByVector(Vector3 vec)
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

            return _globalTransform.Translation;
        }

        /// <summary>
        /// Moves children of a node by a Vector3 recurrently.
        /// </summary>
        /// <param name="vec"> Vector3 by which the children are moved.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Scales the node and its children.
        /// </summary>
        /// <param name="newScale"> Vector3 that is the new Scale to be set.</param>
        public void ScaleNode(Vector3 newScale)
        {
            _globalTransform.Scale = newScale;
            _relativeTransform.Scale = newScale;
#if DEBUG
            _bounds.Width = _baseHeight * newScale.X;
            _bounds.Height = _baseHeight * newScale.Y;
#endif
            foreach (var child in _children)
            {
                var currentScaleGlobal = child.ScaleChildren(newScale);
            }
        }

        /// <summary>
        /// Scales children of given node.
        /// </summary>
        /// <param name="newScale"> Vector3 that is the new Scale to be set.</param>
        /// <returns></returns>
        public Vector3 ScaleChildren(Vector3 newScale)
        {
            _globalTransform.Scale = newScale;
#if DEBUG
            _bounds.Width = _baseHeight * newScale.X;
            _bounds.Height = _baseHeight * newScale.Y;
#endif
            foreach (var child in _children)
            {
                var currentScaleGlobal = child.ScaleChildren(newScale);
            }
            return _globalTransform.Scale;
        }

        #endregion

        #region Lifespan methods

        /// <summary>
        /// This method is called on object awakening.
        /// </summary>
        public void Awake()
        {
            _bounds.X = _globalTransform.Translation.X;
            _bounds.Y = _globalTransform.Translation.Y;
            _rotation = 0.0f;

            foreach (var child in _children) { 
                child.Awake();
            }

            foreach (var comp in _components)
            {
                comp.Awake();
            }

            _activeComponents = _components.Where(x => x._isActive);
            _drawableComponents = _activeComponents.Where(x => x._isDrawable);
        }


        /// <summary>
        /// This method is called every frame.
        /// </summary>
        public void Update()
        {
            if (_id != 0)
            {
                // do stuff, not for root
            }
            

            foreach (var child in _children)
            {
                child.Update();
            }

            foreach (var comp in _activeComponents)
            {
                comp.Update();
            }

        }

        /// <summary>
        /// This method should be called after updating node.
        /// </summary>
        public void Draw()
        {
            var center = GetCenterOfBounds();
            Raylib.DrawRectanglePro(_bounds, new Vector2(center.X, center.Z), _rotation, _boundsColor);

            foreach (var comp in _drawableComponents)
            {
                comp.Draw();
            }
        }

        /// <summary>
        /// Returns center of bounds rectangle.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCenterOfBounds()
        {
            return new Vector3(_globalTransform.Translation.X + (_bounds.Width / 2), _globalTransform.Translation.Y + (_bounds.Height / 2), 0.0f); // 0.0f for Z currently
        }

        #endregion
    }
}
