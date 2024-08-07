using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF2Clone.Base
{
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
