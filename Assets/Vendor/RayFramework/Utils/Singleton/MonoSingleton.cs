using UnityEngine;

namespace RayStudio.UtilScripts.Singleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance != null)
                        DontDestroyOnLoad(_instance.gameObject);
                }

                if (_instance != null) return _instance;

                var go = new GameObject(typeof(T) + "_Singleton");
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);

                return _instance;
            }
        }

        public static T GetMonoInstance()
        {
            if (_instance != null) return _instance;

            if (Application.isPlaying)
            {
                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
                _instance = this as T;
        }

        public void Init()
        {
            InitSingleton();
        }

        public void Free()
        {
            FreeSingleton();
            _instance = null;
            DestroyObject(gameObject);
        }

        protected virtual void InitSingleton()
        {
        }

        protected virtual void FreeSingleton()
        {
        }
    }
}