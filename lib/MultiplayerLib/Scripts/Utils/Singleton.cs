namespace Multiplayer.Utils
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                return _instance;
            }
        }

        public Singleton()
        {
            if (_instance != null)
                return;
            
            _instance = (T) this;
            
            _instance.Start();
        }
        
        ~Singleton()
        {
            if (_instance == this)
                OnDestroy();
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
        }
    }
}