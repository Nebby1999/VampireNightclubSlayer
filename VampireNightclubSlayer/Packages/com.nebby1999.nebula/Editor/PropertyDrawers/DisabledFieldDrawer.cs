using UnityEditor;
using UnityEngine;

namespace Nebula.Editor
{
    [CustomPropertyDrawer(typeof(DisabledFieldAttribute))]
    public class DisabledFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}