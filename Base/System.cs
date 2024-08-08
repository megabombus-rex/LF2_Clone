using LF2Clone.Common;

namespace LF2Clone.Base
{
    public class System<T> : Singleton<T> where T : class, new()
    {
        private int _id;
        private string _name;

        public void Test()
        {

        }
    }
}
