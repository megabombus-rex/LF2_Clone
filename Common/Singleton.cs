namespace LF2Clone.Common
{
    // lifetime until destroyed, systems should be created with this
    public class Singleton<T> where T : class, new()
    {
        protected static T _instance = null;
        protected static readonly object _padlock = new object();

        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
        }
    }
}
