namespace LF2Clone.Base
{
    // Game objects that work as a tree graph
    public class Node
    {
        private Node parent;

        private List<Node> children;

        public Node()
        {
            parent = this;
            children = new List<Node>();
        }
    }
}
