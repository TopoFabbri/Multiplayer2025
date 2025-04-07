using System;

namespace Utils
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly Lazy<T> _instance = new(() => new T());

        public static T Instance => _instance.Value;

        protected Singleton()
        {
            if (_instance.IsValueCreated)
            {
                throw new InvalidOperationException("An instance of this singleton already exists!");
            }
        }
    }
}