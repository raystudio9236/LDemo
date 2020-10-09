namespace RayStudio.UtilScripts.Singleton
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static readonly object LockObject = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    lock (LockObject)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }

                return _instance;
            }
        }

        public void Init()
        {
        }

        public void Free()
        {
            FreeSingleton();
            _instance = null;
        }

        protected virtual void InitSingleton()
        {
        }

        protected virtual void FreeSingleton()
        {
        }
    }
}