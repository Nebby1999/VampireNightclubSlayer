using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nebula.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ForcePrefabAttribute))]
    public class ForcePrefabDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement element = new VisualElement()
            {
                name = "ForcePrefabDrawer"
            };

            PropertyField field = new PropertyField(property);
            field.RegisterValueChangeCallback(CheckForPrefab);
            element.Add(field);
            return element;
        }

        private void CheckForPrefab(SerializedPropertyChangeEvent evt)
        {
            var prop = evt.changedProperty;

            var value = prop.objectReferenceValue;
            if (!value)
                return;

            if(PrefabUtility.IsPartOfPrefabAsset(value))
            {
                return;
            }

            Debug.Log($"Value for property {prop.displayName} must be part of a Prefab Asset.");
            prop.objectReferenceValue = null;
            prop.serializedObject.ApplyModifiedProperties();
        }
    }
}
