#if SMART_ADDRESSER && ADDRESSABLES
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Nebula.Editor.SmartAddresser.AssetFilters
{
    [Serializable]
    [AssetFilter("Nebula/Has Component", "Has Component")]
    public class HasComponent : AssetFilterBase
    {
        [SerializeField] private TypeReferenceListableProperty _type = new TypeReferenceListableProperty();
        [SerializeField] private bool _matchWithDerivedComponents = true;

        private Dictionary<string, HashSet<Component>> _pathToPrefabComponents = new();
        private List<Type> _componentTypes = new List<Type>();

        public TypeReferenceListableProperty Type => _type;

        public bool MatchWithDerivedTypes
        {
            get => _matchWithDerivedComponents;
            set => _matchWithDerivedComponents = value;
        }

        public override void SetupForMatching()
        {
            _pathToPrefabComponents.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t: prefab", new[] { "Assets/" }))
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (!prefab)
                    continue;

                _pathToPrefabComponents.Add(prefabPath, new HashSet<Component>(prefab.GetComponents<Component>()));
            }

            _componentTypes.Clear();
            foreach (var typeRef in _type)
            {
                if (typeRef == null)
                    continue;
                if (!typeRef.IsValid())
                    continue;

                var type = System.Type.GetType(typeRef.AssemblyQualifiedName);
                _componentTypes.Add(type);
            }
        }

        public override string GetDescription()
        {
            var result = new StringBuilder();
            var elementCount = 0;
            foreach (var type in _componentTypes)
            {
                if (type == null || string.IsNullOrEmpty(type.Name))
                    continue;

                if (elementCount >= 1)
                    result.Append(" || ");
                result.Append(type.Name);
                elementCount++;
            }

            if (result.Length >= 1)
            {
                if (elementCount >= 2)
                {
                    result.Insert(0, "( ");
                    result.Append(" )");
                }
                result.Insert(0, "Component: ");
            }

            if (MatchWithDerivedTypes)
            {
                result.Append(" and derived components");
            }
            return result.ToString();
        }

        public override bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            if (assetType == null)
                return false;

            if (!_pathToPrefabComponents.TryGetValue(assetPath, out HashSet<Component> components))
            {
                return false;
            }

            for (var i = 0; i < _componentTypes.Count; i++)
            {
                Type componentType = _componentTypes[i];
                foreach (Component component in components)
                {
                    Type componentType2 = component.GetType();

                    if (componentType2 == null)
                        continue;

                    if (componentType == componentType2)
                    {
                        return true;
                    }

                    if (_matchWithDerivedComponents && componentType2.IsSubclassOf(componentType))
                        return true;
                }
            }
            return false;
        }

    }
}
#endif