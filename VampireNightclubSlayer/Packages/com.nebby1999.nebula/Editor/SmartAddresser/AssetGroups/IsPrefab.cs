#if SMART_ADDRESSER && ADDRESSABLES
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nebula.Editor.SmartAddresser.AssetFilters
{
    [Serializable]
    [AssetFilter("Nebula/Is Prefab", "Is Prefab")]
    public class IsPrefab : AssetFilterBase
    {
        public List<string> prefabPaths = new();
        public override string GetDescription()
        {
            return "Is Prefab";
        }

        public override bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            if (isFolder)
                return false;

            if (assetType != typeof(GameObject))
                return false;

            return prefabPaths.Contains(assetPath);
        }

        public override void SetupForMatching()
        {
            prefabPaths.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t: prefab", new[] { "Assets\\" }))
            {
                prefabPaths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }
        }
    }
}
#endif