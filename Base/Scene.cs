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
            _root.Awake();
        }

        public void Update()
        {
            _root.Update();
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
            var nextID = _nodes.Max(x => x._id) + 1;
            var name = _defaultRootName;
            name = GetNextValidNodeName(parent, name);
            _nodes.Add(new Node(parent, nextID, name));
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
                node._name = GetNextValidNodeName(node.GetParent()!, newName);
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
                throw new ArgumentException("Cannot reparent root node.");
            }

            if (parent._id == nodeId)
            {
                throw new ArgumentException("Cannot reparent node to itself.");
            }

            try
            {
                var node = _nodes.First(x => x._id == nodeId);
                node._name = GetNextValidNodeName(parent, _name);
                node.ReparentCurrentNode(parent);
            }
            catch
            {
                throw;
            }
        }

        // Returns the next valid node name based on other children names
        private string GetNextValidNodeName(Node parent, string newNodeName)
        {
            int nextNameNumber = 1;
            var childrenCount = parent.GetChildren().Count;
            if (childrenCount > 0)
            {
                while (parent.GetChildren().Any(x => x._name == newNodeName))
                {
                    if (nextNameNumber > 1)
                    {
                        newNodeName = newNodeName.Substring(0, newNodeName.Length - 1);
                    }
                    newNodeName += nextNameNumber.ToString();
                    nextNameNumber++;
                }
            }

            return newNodeName;
        }

    }
}
