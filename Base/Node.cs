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

        private Lazy<List<Node>> child;

        public Node()
        {
            parent = this;
        }
    }
}
