using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Nebula.Editor
{
    public static class IMGUIUtil
    {
        private delegate object FieldDrawHandler(FieldInfo fieldInfo, object value);
        private static readonly Dictionary<Type, FieldDrawHandler> _typeDrawers = new Dictionary<Type, FieldDrawHandler>
        {
            [typeof(bool)] = (fieldInfo, value) => EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(fieldInfo.Name), (bool)value),
            [typeof(long)] = (fieldInfo, value) => EditorGUILayout.LongField(ObjectNames.NicifyVariableName(fieldInfo.Name), (long)value),
            [typeof(int)] = (fieldInfo, value) => EditorGUILayout.IntField(ObjectNames.NicifyVariableName(fieldInfo.Name), (int)value),
            [typeof(float)] = (fieldInfo, value) => EditorGUILayout.FloatField(ObjectNames.NicifyVariableName(fieldInfo.Name), (float)value),
            [typeof(double)] = (fieldInfo, value) => EditorGUILayout.DoubleField(ObjectNames.NicifyVariableName(fieldInfo.Name), (double)value),
            [typeof(string)] = (fieldInfo, value) => EditorGUILayout.TextField(ObjectNames.NicifyVariableName(fieldInfo.Name), (string)value),
            [typeof(Vector2)] = (fieldInfo, value) => EditorGUILayout.Vector2Field(ObjectNames.NicifyVariableName(fieldInfo.Name), (Vector2)value),
            [typeof(Vector3)] = (fieldInfo, value) => EditorGUILayout.Vector3Field(ObjectNames.NicifyVariableName(fieldInfo.Name), (Vector3)value),
            [typeof(Color)] = (fieldInfo, value) => EditorGUILayout.ColorField(ObjectNames.NicifyVariableName(fieldInfo.Name), (Color)value),
            [typeof(Color32)] = (fieldInfo, value) => (Color32)EditorGUILayout.ColorField(ObjectNames.NicifyVariableName(fieldInfo.Name), (Color32)value),
            [typeof(AnimationCurve)] = (fieldInfo, value) => EditorGUILayout.CurveField(ObjectNames.NicifyVariableName(fieldInfo.Name), (AnimationCurve)value ?? new AnimationCurve()),
        };

        public static void DrawCheckableProperty(SerializedProperty prop, Action propertyChangedAction, bool includeChildren = false, GUIContent label = null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(prop, label ?? GetLabelFromProperty(prop), includeChildren);
            prop.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                propertyChangedAction();
            }
        }
        public static void DrawCheckableProperty(SerializedProperty prop, Action<SerializedProperty> propertyChangedAction, bool includeChildren = true, GUIContent label = null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(prop, label ?? GetLabelFromProperty(prop), includeChildren);
            prop.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                propertyChangedAction(prop);
            }
        }

        public static GUIContent GetLabelFromProperty(SerializedProperty prop)
        {
            return new GUIContent(prop.displayName, prop.tooltip);
        }

        public static bool ConditionalButton(bool condition, string text, string tooltip = null, Texture texture = null)
        {
            return ConditionalButton(() => condition, text, tooltip, texture);
        }

        public static bool ConditionalButton(bool condition, GUIContent label)
        {
            return ConditionalButton(() => condition, label);
        }

        public static bool ConditionalButton(Func<bool> condition, string text, string tooltip = null, Texture texture = null)
        {
            return ConditionalButton(condition, new GUIContent(text, texture, tooltip));
        }

        public static bool ConditionalButton(Func<bool> condition, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(!condition());
            bool buttonVal = GUILayout.Button(label);
            EditorGUI.EndDisabledGroup();
            return buttonVal;
        }

        public static bool ConditionalButtonAction(Action action, bool condition, string text, string tooltip = null, Texture texture = null)
        {
            return ConditionalButtonAction(action, () => condition, text, tooltip, texture);
        }
        public static bool ConditionalButtonAction(Action action, bool condition, GUIContent content)
        {
            return ConditionalButtonAction(action, () => condition, content);
        }

        public static bool ConditionalButtonAction(Action action, Func<bool> condition, string text, string tooltip = null, Texture texture = null)
        {
            return ConditionalButtonAction(action, condition, new GUIContent(text, texture, tooltip));
        }

        public static bool ConditionalButtonAction(Action action, Func<bool> condition, GUIContent content)
        {
            var val = ConditionalButton(condition, content);
            if (val)
                action();
            return val;
        }
        public static bool ButtonAction(Action action, string text, string tooltip = null, Texture texture = null, params GUILayoutOption[] options) => ButtonAction(action, new GUIContent(text, texture, tooltip), options);

        public static bool ButtonAction(Action action, GUIContent label, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(label, options))
            {
                action();
                return true;
            }
            return false;
        }

        public static bool TryDrawFieldFromFieldInfo(FieldInfo fieldInfo, object objectInstance)
        {
            try
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType))
                {
                    var objectValue = (UnityEngine.Object)fieldInfo.GetValue(objectInstance);
                    EditorGUILayout.ObjectField(new GUIContent(ObjectNames.NicifyVariableName(fieldInfo.Name)), objectValue, fieldInfo.FieldType, false);
                    return true;
                }

                if (_typeDrawers.TryGetValue(fieldInfo.FieldType, out var drawer))
                {
                    drawer(fieldInfo, fieldInfo.GetValue(objectInstance));
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}