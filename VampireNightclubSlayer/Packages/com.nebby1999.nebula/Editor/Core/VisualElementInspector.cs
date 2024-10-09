using UnityEngine.UIElements;

namespace Nebula.Editor
{
    public class VisualElementInspector<T> : Inspector<T> where T : UnityEngine.Object
    {
        protected virtual bool ValidateUXMLPath(string path)
        {
            return path.Contains(Constants.PACKAGE_NAME);
        }

        public sealed override VisualElement CreateInspectorGUI()
        {
            serializedObject.ApplyModifiedProperties();
            var rootElement = VisualElementTemplateFinder.GetTemplateInstance(GetType().Name, null, ValidateUXMLPath);
            ModifyRootElement(rootElement);
            return rootElement;
        }

        public sealed override void OnInspectorGUI()
        {
        }

        protected virtual void ModifyRootElement(VisualElement rootElement) { }
    }
}