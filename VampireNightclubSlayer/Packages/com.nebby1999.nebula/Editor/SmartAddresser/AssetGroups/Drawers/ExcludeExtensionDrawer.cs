#if SMART_ADDRESSER && ADDRESSABLES
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;

namespace Nebula.Editor.SmartAddresser.AssetFilters.Drawers
{
    [CustomGUIDrawer(typeof(ExcludeExtension))]
    internal sealed class ExcludeExtensionDrawer : GUIDrawer<ExcludeExtension>
    {
        private TextListablePropertyGUI _listablePropertyGUI;

        public override void Setup(object target)
        {
            base.Setup(target);
            _listablePropertyGUI = new TextListablePropertyGUI(ObjectNames.NicifyVariableName(nameof(Target.excludedExtensions)), Target.excludedExtensions);
        }

        protected override void GUILayout(ExcludeExtension target)
        {
            _listablePropertyGUI.DoLayout();
        }
    }
}
#endif