using UnityEngine;

namespace Nebula
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T instance { get; private set; }
        protected virtual bool destroyIfDuplicate { get; } = false;

        protected virtual void OnEnable()
        {
            if(instance != null && instance != this)
            {
                Debug.LogWarning($"Duplicate instance of {typeof(T).Name} detected. Only a single instance should exist at a time! " + (destroyIfDuplicate ? "Destroying Duplicate." : "Replacing instance with new one."), this);

                if(destroyIfDuplicate)
                {
                    DestroySelf();
                    return;
                }
            }
            instance = this as T;
        }

        protected virtual void DestroySelf()
        {
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#else
            Destroy(gameObject);
#endif
        }

        protected virtual void OnDisable()
        {
            if(instance == this)
            {
                instance = null;
            }
        }
    }
}