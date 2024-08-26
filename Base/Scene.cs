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

        public void Update()
        {
            _root.Update();
            foreach (Node node in _nodes)
            {
                node.Draw();
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

            if (!node.TryRemoveNode())
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

        public void RenameNode(int id, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new InvalidDataException("New node name cannot be empty.");
            }

            try
            {
                var node = _nodes.First(x => x._id == id);
                node._name = GetNextValidNodeName(node.GetParent()!, newName);
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
