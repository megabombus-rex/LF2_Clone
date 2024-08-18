using LF2Clone.Misc.Logger;

namespace LF2Clone.Base
{
    public class System<T> where T : class
    {
        protected int _id;
        protected string _name;
        protected ILogger<T>? _logger;

        protected System()
        {
            _id = 0;
            _name = "system";
        }

        public virtual void Setup(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}
