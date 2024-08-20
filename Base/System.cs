using LF2Clone.Misc.Logger;

namespace LF2Clone.Base
{
    public class System<T> where T : class
    {
        protected int _id;
        protected string _name;
        protected ILogger<T> _logger;

        protected System(ILogger<T> logger)
        {
            _id = 0;
            _name = "system";
            _logger = logger;
        }

        public virtual void Setup()
        {
        }

        public virtual void Destroy()
        {
            _logger.Dispose();
        }
    }
}
