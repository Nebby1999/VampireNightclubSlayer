#if USE_ADDRESSABLES && USE_SMART_ADDRESSER
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;

namespace Nebula.Editor.SmartAddresser.AssetFilters.Drawers
{
    [CustomGUIDrawer(typeof(ExcludeAssets))]
    internal sealed class ExcludeAssetsDrawer : GUIDrawer<ExcludeAssets>
    {
        private ObjectListablePropertyGUI _listablePropertyGUI;

        public override void Setup(object target)
        {
            base.Setup(target);
            _listablePropertyGUI = new ObjectListablePropertyGUI(ObjectNames.NicifyVariableName(nameof(Target.excludedAssets)), Target.excludedAssets, typeof(UnityEngine.Object), false);
        }

        protected override void GUILayout(ExcludeAssets target)
        {
            _listablePropertyGUI.DoLayout();
        }
    }
}
#endif