namespace LF2Clone.Base
{
    public class Scene
    {
        public int _id;
        public string _name;
        public Node _root;

        public Scene(int id, string name)
        {
            _id = id;
            _name = name;
            _root = new Node();
        }
    }
}
