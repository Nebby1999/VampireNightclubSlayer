using Nebula.Editor.VisualElements;
using Nebula.Serialization;
using UnityEditor;
using UnityEngine.UIElements;

namespace Nebula.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SerializedFieldCollection))]
    public class SerializedFieldCollectionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var instance = new SerializedFieldCollectionElement();
            instance.boundProperty = property;
            return instance;
        }
    }
}