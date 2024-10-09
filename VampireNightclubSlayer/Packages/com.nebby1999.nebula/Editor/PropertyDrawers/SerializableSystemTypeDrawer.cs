using EntityStates;
using Nebula.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nebula.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SerializableSystemType.RequiredBaseTypeAttribute))]
    public class SerializableSystemTypeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new IMGUIContainer(() =>
            {
                var typeNameProperty = property.FindPropertyRelative("_typeName");
                var typeName = Type.GetType(typeNameProperty.stringValue)?.Name ?? "No State Type";
                var baseType = attribute is SerializableSystemType.RequiredBaseTypeAttribute att ? att.type : null;
                var label = new GUIContent(property.displayName);

                var rect = EditorGUILayout.BeginHorizontal();
                GUILayout.Label(label, GUILayout.ExpandWidth(false));
                if(EditorGUILayout.DropdownButton(new GUIContent(typeName, typeNameProperty.stringValue), FocusType.Passive))
                {
                    Rect labelRect = GUILayoutUtility.GetLastRect();

                    var rectToUse = new Rect
                    {
                        x = rect.x + labelRect.width,
                        y = rect.y,
                        height = rect.height,
                        width = rect.width - labelRect.width
                    };

                    var dropdown = new InheritingTypeSelectDropdown(new AdvancedDropdownState(), baseType);
                    dropdown.onItemSelected += (item) =>
                    {
                        property.FindPropertyRelative("_typeName").stringValue = item.assemblyQualifiedName;
                        property.serializedObject.ApplyModifiedProperties();
                    };
                    dropdown.Show(rectToUse);
                }
                EditorGUILayout.EndHorizontal();
            });
        }
    }
}