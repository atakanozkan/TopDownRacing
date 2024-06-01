using UnityEngine;


namespace TDR.Patterns
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }

                return _instance;
            }
        }

        public bool isPersistent;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (isPersistent)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                if (_instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}