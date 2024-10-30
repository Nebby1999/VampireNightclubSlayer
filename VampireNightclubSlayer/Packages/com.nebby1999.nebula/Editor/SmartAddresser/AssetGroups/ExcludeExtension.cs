#if USE_ADDRESSABLES && USE_SMART_ADDRESSER
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.ValidationError;
using SmartAddresser.Editor.Foundation.ListableProperty;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Nebula.Editor.SmartAddresser.AssetFilters
{
    [Serializable]
    [AssetFilter("Nebula/Exclude Extension Filter", "Exclude Extension Filter")]
    public class ExcludeExtension : AssetFilterBase
    {
        public StringListableProperty excludedExtensions = new StringListableProperty();
        private List<string> _extensions = new List<string>();
        public override void SetupForMatching()
        {
            _extensions.Clear();
            foreach (var extension in excludedExtensions)
            {
                if (string.IsNullOrEmpty(extension))
                    continue;

                var extensionName = extension;
                if (!extensionName.StartsWith("."))
                {
                    extensionName = $".{extension}";
                }
                _extensions.Add(extensionName);
            }
        }

        public override bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            if (string.IsNullOrEmpty(assetPath))
                return false;

            foreach (var extension in _extensions)
            {
                if (assetPath.EndsWith(extension, StringComparison.Ordinal))
                {
                    return false;
                }
            }
            return true;
        }

        public override string GetDescription()
        {
            var result = new StringBuilder();
            var elementCount = 0;
            foreach (var extension in _extensions)
            {
                if (string.IsNullOrEmpty(extension))
                    continue;

                if (elementCount >= 1)
                    result.Append(" || ");

                result.Append(extension);
                elementCount++;
            }

            if (result.Length >= 1)
            {
                if (elementCount >= 2)
                {
                    result.Insert(0, "( ");
                    result.Append(" )");
                }

                result.Insert(0, "Excluding Extension: ");
            }

            return result.ToString();
        }

        public override bool Validate(out AssetFilterValidationError error)
        {
            error = null;
            return true;
        }
    }
}
#endif