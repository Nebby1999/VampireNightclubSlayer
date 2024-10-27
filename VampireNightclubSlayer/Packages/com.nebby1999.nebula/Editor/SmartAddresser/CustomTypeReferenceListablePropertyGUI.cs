#if USE_ADDRESSABLES && USE_SMART_ADDRESSER
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.ListableProperty;
using System;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;

namespace Nebula.Editor.SmartAddresser
{
    public class CustomTypeReferenceListablePropertyGUI : ListablePropertyGUI<TypeReference>
    {
        private const string TEMP_CONTROL_NAME = "CustomTypeReferenceListablePropertyGUI.TempControl";
        public CustomTypeReferenceListablePropertyGUI(string displayName, ListableProperty<TypeReference> list)
            : base(displayName, list, (rect, label, value, onValueChanged) =>
            {
                var buttonText = value.Name;
                if (string.IsNullOrEmpty(buttonText))
                    buttonText = "-";

                var propertyRect = EditorGUI.PrefixLabel(rect, new GUIContent(label));
                GUI.SetNextControlName(TEMP_CONTROL_NAME);
                if (EditorGUI.DropdownButton(propertyRect, new GUIContent(buttonText), FocusType.Passive))
                {
                    GUI.FocusControl(TEMP_CONTROL_NAME);
                    var dropdown = new InheritingTypeSelectDropdown(new AdvancedDropdownState(), typeof(Component));

                    void OnItemSelected(InheritingTypeSelectDropdown.Item item)
                    {
                        onValueChanged(TypeReference.Create(Type.GetType(item.assemblyQualifiedName)));
                        dropdown.onItemSelected -= OnItemSelected;
                    }

                    dropdown.onItemSelected += OnItemSelected;
                    dropdown.Show(propertyRect);
                }
            }, () => new TypeReference())
        {
        }
    }
}
#endif