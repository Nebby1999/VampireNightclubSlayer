using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Nebula.Editor.Inspectors
{
    public abstract class StateMachineInspector<T> : Inspector<T> where T : StateMachine
    {
        private bool _foldout;
        private Vector2 _scrollPos;
        private string searchBar
        {
            get => _searchBar;
            set
            {
                if (_searchBar != value)
                {
                    _searchBar = value;
                    FilterInstanceFields();
                }
            }
        }
        private string _searchBar;
        private Type currentInspectedStateType
        {
            get => _currentInspectedStateType;

            set
            {
                if (_currentInspectedStateType != value)
                {
                    _currentInspectedStateType = value;
                    GetInstanceFields();
                    FilterInstanceFields();
                }
            }
        }
        private Type _currentInspectedStateType;
        private FieldInfo[] _inspectedTypeInstanceFields = Array.Empty<FieldInfo>();
        private FieldInfo[] _filteredTypeInstanceFields = Array.Empty<FieldInfo>();

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.Add(new IMGUIContainer(IMGUIHandler));
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            var m_ScriptElement = root.Q<PropertyField>("PropertyField:m_Script");
            root.Remove(m_ScriptElement);
            return root;
        }

        private void IMGUIHandler()
        {
            currentInspectedStateType = targetType.currentState?.GetType();
            string text = currentInspectedStateType?.AssemblyQualifiedName ?? "Not in Playmode or Null";

            EditorGUILayout.BeginVertical();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(new GUIContent($"Current State:"), text);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginVertical("box");
            _foldout = EditorGUILayout.Foldout(_foldout, "Instance Fields", true);
            if (_foldout)
            {
                EditorGUI.indentLevel++;
                DrawInspectedInstanceFields();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
        private void DrawInspectedInstanceFields()
        {
            if (currentInspectedStateType == null)
            {
                EditorGUILayout.HelpBox("Current state is null", MessageType.Warning);
                return;
            }


            List<FieldInfo> undrawableFields = new List<FieldInfo>();
            searchBar = EditorGUILayout.DelayedTextField(new GUIContent("Search"), searchBar);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, true, true, GUILayout.Width(Screen.width), GUILayout.Height(100));
            EditorGUI.BeginDisabledGroup(true);
            foreach (FieldInfo info in _filteredTypeInstanceFields)
            {
                if (!IMGUIUtil.TryDrawFieldFromFieldInfo(info, targetType.currentState))
                {
                    undrawableFields.Add(info);
                }
            }
            EditorGUI.EndDisabledGroup();

            if (undrawableFields.Count > 0)
            {
                DrawUndrawableFields(undrawableFields);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawUndrawableFields(IEnumerable<FieldInfo> fields)
        {
            var text = fields.Select(x => $"{x.Name}, {x.FieldType}").ToArray();
            EditorGUILayout.HelpBox($"The following {text.Length} could not be drawn:\n {string.Join("\n", text)}", MessageType.Warning);
        }
        private void GetInstanceFields()
        {
            if (currentInspectedStateType == null)
                return;

            List<FieldInfo> instanceFields = new List<FieldInfo>();
            var currentType = currentInspectedStateType;
            while (currentType.BaseType != null)
            {
                instanceFields.AddRange(currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
                currentType = currentType.BaseType;
            }

            _inspectedTypeInstanceFields = instanceFields.OrderBy(x => x.FieldType.Name).ToArray();
        }

        private void FilterInstanceFields()
        {
            if (searchBar.IsNullOrWhiteSpace())
            {
                _filteredTypeInstanceFields = _inspectedTypeInstanceFields;
                return;
            }

            _filteredTypeInstanceFields = _inspectedTypeInstanceFields.Where(f => f.Name.Contains(searchBar, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        public override bool RequiresConstantRepaint()
        {
            return _foldout;
        }
    }
}