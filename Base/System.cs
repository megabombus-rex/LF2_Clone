using LF2Clone.Common;
using LF2Clone.Misc.Logger;

namespace LF2Clone.Base
{
    public class System<T> : Singleton<T> where T : class, new()
    {
        protected int _id;
        protected string _name;
        protected ILogger? _logger;

        protected System()
        {
            _id = 0;
            _name = "system";
        }

        public virtual void Setup(ILogger logger)
        {
            _logger = logger;
        }
    }
}
