using UnityEditor;
using UnityEngine;


namespace Nebula.Editor
{
    [CustomPropertyDrawer(typeof(ValueLabelAttribute))]
    public class ValueLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    EditorGUI.PropertyField(position, property, new GUIContent
                    {
                        image = label.image,
                        text = label.text + " | " + GetValueAsString(property.objectReferenceValue),
                        tooltip = label.tooltip,
                    });
                    break;
                default:
                    EditorGUI.PropertyField(position, property, new GUIContent
                    {
                        image = label.image,
                        text = label.text + " | " + property.boxedValue.ToString(),
                        tooltip = label.tooltip,
                    });
                    break;
            }
            EditorGUI.EndProperty();
        }

        private string GetValueAsString(Object obj)
        {
            if (!obj)
                return "Null";

            SerializedObject serializedObject = new SerializedObject(obj);
            var att = (ValueLabelAttribute)attribute;
            SerializedProperty prop = serializedObject.FindProperty(att.propertyName);
            return prop?.boxedValue?.ToString() ?? "Null";
        }
    }
}