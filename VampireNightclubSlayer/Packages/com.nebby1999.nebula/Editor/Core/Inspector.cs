using UnityEngine;

namespace Nebula.Editor
{
    public abstract class Inspector<T> : UnityEditor.Editor where T : Object
    {
        protected T targetType => target as T;

        protected virtual void OnDisable()
        {
            if(serializedObject != null && serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}