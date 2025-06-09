using UnityEngine;

namespace Utils
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        private static MonoBehaviourSingleton<T> _instance;

        public static T Instance
        {
            get 
            {
                if (!_instance)
                    _instance = FindFirstObjectByType<MonoBehaviourSingleton<T>>();

                return (T)_instance;
            }
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Awake()
        {
            if (_instance)
                Destroy(gameObject);

            _instance = this;

            Initialize();
        }
    }
}
