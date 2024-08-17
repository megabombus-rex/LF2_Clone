namespace LF2Clone.Base
{
    // Game objects that work as a tree graph
    public class Node
    {
        // ids should not repeat on one scene as nodes should be destroyed on scene unload
        public int _id;
        public string _name;

        private Node _parent;
        private List<Node> _children;

        // only one component of each type is permitted
        private List<Component> _components;

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
