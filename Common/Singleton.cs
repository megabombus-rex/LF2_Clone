namespace LF2Clone.Common
{
    // lifetime until destroyed, systems should be created with this
    public class Singleton<T> where T : class, new()
    {
        protected static T instance = null;
        protected static readonly object padlock = new object();

        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                    return instance;
                }
            }
        }
    }
}
