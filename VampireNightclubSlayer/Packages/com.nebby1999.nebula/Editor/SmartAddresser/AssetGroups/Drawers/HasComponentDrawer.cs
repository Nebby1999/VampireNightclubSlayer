#if USE_ADDRESSABLES && USE_SMART_ADDRESSER
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace Nebula.Editor.SmartAddresser.AssetFilters.Drawers
{
    [CustomGUIDrawer(typeof(HasComponent))]
    public class HasComponentDrawer : GUIDrawer<HasComponent>
    {
        private CustomTypeReferenceListablePropertyGUI _listablePropertyGUI;

        public override void Setup(object target)
        {
            base.Setup(target);
            _listablePropertyGUI = new CustomTypeReferenceListablePropertyGUI("Component Type", Target.Type);
        }
        protected override void GUILayout(HasComponent target)
        {
            target.MatchWithDerivedTypes = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(Target.MatchWithDerivedTypes)), Target.MatchWithDerivedTypes);
            _listablePropertyGUI.DoLayout();
        }
    }
}
#endif