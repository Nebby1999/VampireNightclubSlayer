using Nebula.Editor.VisualElements;
using Nebula.Serialization;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nebula.Editor
{
    public abstract class StateConfigurationInspector
        <TStateConfiguration> : Inspector<TStateConfiguration> where TStateConfiguration : StateConfiguration
    {
        private SerializedProperty stateTypeProperty;
        private PropertyField stateTypeToConfigPropertyField;

        private SerializedProperty fieldCollectionProperty;
        private SerializedFieldCollectionElement serializedFieldCollectionElement;

        private void OnEnable()
        {
            stateTypeProperty = serializedObject.FindProperty(GetStateTypeToConfigPropertyName());
            fieldCollectionProperty = serializedObject.FindProperty(nameof(StateConfiguration.fieldCollection));
        }

        protected abstract string GetStateTypeToConfigPropertyName();

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            stateTypeToConfigPropertyField = new PropertyField();
            stateTypeToConfigPropertyField.bindingPath = stateTypeProperty.propertyPath;
            root.Add(stateTypeToConfigPropertyField);
            stateTypeToConfigPropertyField.TrackPropertyValue(stateTypeProperty.FindPropertyRelative("_typeName"), UpdateFieldCollection);

            serializedFieldCollectionElement = new SerializedFieldCollectionElement();
            serializedFieldCollectionElement.boundProperty = fieldCollectionProperty;
            serializedFieldCollectionElement.typeBeingSerialized = GetStateType();
            root.Add(serializedFieldCollectionElement);
            return root;
        }

        private void UpdateFieldCollection(SerializedProperty prop)
        {
            serializedFieldCollectionElement.typeBeingSerialized = GetStateType();
        }

        private Type GetStateType()
        {
            return Type.GetType(stateTypeProperty.FindPropertyRelative("_typeName").stringValue);
        }
    }
}
