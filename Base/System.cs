using LF2Clone.Common;

namespace LF2Clone.Base
{
    public class System<T> : Singleton<T> where T : class, new()
    {
        protected int _id;
        protected string _name;

        protected System()
        {
            _id = 0;
            _name = "system";
        }

        public void Test()
        {

        }
    }
}
