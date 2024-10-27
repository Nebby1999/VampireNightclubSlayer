#if USE_ADDRESSABLES && USE_SMART_ADDRESSER
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Models.Shared;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.Shared;
using System;

namespace Nebula.Editor.SmartAddresser.AddressProviders
{
    [Serializable]
    public class SimplifiedAssetPath : IAddressProvider
    {
        public string GetDescription()
        {
            return "Source: SimplifiedAssetPath";
        }

        public string Provide(string assetPath, Type assetType, bool isFolder)
        {
            return assetPath.StartsWith("Assets/") ? assetPath.Substring("Assets/".Length) : assetPath;
        }

        public void Setup()
        {
        }
    }
}
#endif