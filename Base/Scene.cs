using LF2Clone.Exceptions;
using LF2Clone.Misc.Helpers;

namespace LF2Clone.Base
{
    public class Scene
    {
        public int _id;
        public string _defaultRootName;
        public string _name;
        public Node _root;
        public List<Node> _nodes;

        public Scene(int id, string name)
        {
            _id = id;
            _name = name;
            _root = new Node();
            _nodes = new List<Node>
            {
                _root
            };
            _defaultRootName = "Node"; // move to config, add as parameter
        }

        public void Awake()
        {
            foreach (Node node in _nodes)
            {
                node.Awake();
            }
        }

        public void Update()
        {
            foreach (Node node in  _nodes)
            {
                node.Update();
            }
        }

        public void Draw()
        {
            foreach (Node node in _nodes)
            {
                node.Draw();
            }
        }

        public Node GetNodeById(int nodeId)
        {
            try
            {
                return _nodes.First(x => x._id == nodeId);
            }
            catch
            {
                throw;
            }
        }

        // Returns true if the node with nodeId (param) was successfully removed, false otherwise.
        public bool TryRemoveNode(int nodeId)
        {
            var node = _nodes.FirstOrDefault(x => x._id == nodeId);

            if(node == null)
            {
                return false;
            }

            if (!node.TryRemoveNodeFromParent())
            {
                return false;
            }

            var children = _nodes.Where(x => x._parentId == nodeId).ToList();

            foreach (var child in children)
            {
                _nodes.Remove(child);
            }

            if (!_nodes.Remove(node))
            {
                node.GetParent().AddChild(node); // return to the previous state

                foreach(var child in children)
                {
                    _nodes.Add(child);
                }
                return false;
            }

            return true;
        }

        // Adds a new node to given parent (param).
        public void AddNewNode(Node parent)
        {
            var nextID = NamingHelper.GetNextAvailableId(_nodes.Select(x => x._id));
            var name = _defaultRootName;
            name = NamingHelper.GetNextValidName(parent.GetChildren().Select(x => x._name), name);
            var node = new Node(parent, nextID, name);
            _nodes.Add(node);
            node.SetSceneReference(this);
        }

        // Renames a node selected by nodeId (param) to a newName (param) or next possible name.
        public void RenameNode(int nodeId, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new InvalidDataException("New node name cannot be empty.");
            }

            try
            {
                var node = _nodes.First(x => x._id == nodeId);
                node._name = NamingHelper.GetNextValidName(node.GetParent()!.GetChildren().Select(x => x._name), newName);
            }
            catch
            {
                throw;
            }
        }

        // Reparents node indicated by nodeId (param) to a parent node.
        public void ReparentNode(Node parent, int nodeId)
        {
            if (nodeId == 0)
            {
                // It should not be possible when the engine is finished.
                throw new NodeReparentingException("Cannot reparent root node.");
            }

            if (parent._id == nodeId)
            {
                throw new NodeReparentingException("Cannot reparent node to itself.");
            }

            try
            {
                var node = _nodes.First(x => x._id == nodeId);
                node._name = NamingHelper.GetNextValidName(parent.GetChildren().Select(x => x._name), node._name);
                node.ReparentCurrentNode(parent);
            }
            catch
            {
                throw;
            }
        }
    }
}
