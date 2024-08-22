﻿namespace LF2Clone.Base
{
    public class Scene
    {
        public int _id;
        public string _defaultRootName;
        public string _name;
        public Node _root;
        public List<Node> _nodes;

        // read serialized scene with scene loader
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

        public void AddNewNode(Node parent)
        {
            var nextID = _nodes.Max(x => x._id) + 1;
            var name = _defaultRootName;

            int nextNameNumber = 1;
            if(parent.GetChildren().Count > 0)
            {
                while (parent.GetChildren().Any(x => x._name == name))
                {
                    if (nextNameNumber > 1)
                    {
                        name = name.Substring(0, name.Length - 1);
                    }
                    name += nextNameNumber.ToString();
                    nextNameNumber++;
                }
            }

            var node = new Node(parent, nextID, name);
            _nodes.Add(node);
        }
    }
}
