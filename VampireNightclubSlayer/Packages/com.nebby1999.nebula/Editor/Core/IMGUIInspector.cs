using UnityEngine.UIElements;

namespace Nebula.Editor
{
    public abstract class IMGUIInspector<T> : Inspector<T> where T : UnityEngine.Object
    {
        public sealed override VisualElement CreateInspectorGUI()
        {
            return null;
        }

        public sealed override void OnInspectorGUI()
        {
            DrawGUI();
            serializedObject.ApplyModifiedProperties();
        }

        public abstract void DrawGUI();
    }
}