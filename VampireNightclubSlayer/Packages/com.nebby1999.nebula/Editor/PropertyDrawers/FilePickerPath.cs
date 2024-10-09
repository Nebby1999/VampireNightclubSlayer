using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nebula.Editor
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FilePickerPath : PropertyAttribute
    {
        public string title { get; set; }
        public string defaultName { get; set; }
        public string extension { get; set; }
        [CustomPropertyDrawer(typeof(FilePickerPath))]
        private class Drawer : PropertyDrawer
        {
            public override VisualElement CreatePropertyGUI(SerializedProperty property)
            {
                var attribute = this.attribute as FilePickerPath;
                var container = new VisualElement()
                {
                    name = property.propertyPath + "Container",
                };
                container.style.flexDirection = FlexDirection.Row;

                var textField = new TextField(property.name);
                textField.BindProperty(property);
                textField.style.flexGrow = 1;
                container.Add(textField);

                var button = new Button(() =>
                {
                    var fileName = EditorUtility.SaveFilePanel(attribute.title, Application.dataPath, attribute.defaultName, attribute.extension);
                    if(!string.IsNullOrEmpty(fileName))
                    {
                        if(fileName.StartsWith(Application.dataPath))
                        {
                            fileName = "Assets/" + fileName.Substring(Application.dataPath.Length + 1);
                        }
                        textField.value = fileName;
                    }
                });

                button.name = property.propertyPath + "FilePathButton";
                button.style.flexGrow = 0;
                button.text = "...";
                container.Add(button);

                return container;
            }
        }
    }
}