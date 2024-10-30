#if USE_ADDRESSABLES && USE_SMART_ADDRESSER
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using System;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using System.Text;
using SmartAddresser.Editor.Foundation.ListableProperty;
using System.Collections.Generic;
using UnityEditor;

namespace Nebula.Editor.SmartAddresser.AssetFilters
{
    [Serializable]
    [AssetFilter("Nebula/Exclude Asset(s) Filter", "Exclude Asset(s) Filter")]
    public class ExcludeAssets : AssetFilterBase
    {
        public ObjectListableProperty excludedAssets = new ObjectListableProperty();
        private List<string> _excludedAssetsPath = new List<string>();
        public override string GetDescription()
        {
            var result = new StringBuilder();
            var elementCount = 0;
            foreach(var item in excludedAssets )
            {
                if (!item)
                    continue;

                if(elementCount >= 1)
                {
                    result.Append(" && ");
                }
                result.Append(item.name);
                elementCount++;
            }

            if(result.Length >= 1)
            {
                if (elementCount >= 2)
                {
                    result.Insert(0, "( ");
                    result.Append(" )");
                }
                result.Insert(0, "Excluding Asset: ");
            }
            return result.ToString();
        }

        public override bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            if (string.IsNullOrEmpty(assetPath))
                return false;

            foreach(var item in _excludedAssetsPath)
            {
                if (assetPath == item)
                    return false;
            }
            return true;
        }

        public override void SetupForMatching()
        {
            _excludedAssetsPath.Clear();
            foreach(var asset in excludedAssets)
            {
                if (!asset)
                    continue;

                _excludedAssetsPath.Add(AssetDatabase.GetAssetPath(asset));
            }
        }

        public override bool Validate(out AssetFilterValidationError error)
        {
            error = null;
            return true;
        }
    }
}
#endif