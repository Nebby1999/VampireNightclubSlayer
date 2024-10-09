#if SMART_ADDRESSER && ADDRESSABLES
using SmartAddresser.Editor.Foundation.CustomDrawers;

namespace Nebula.Editor.SmartAddresser.AssetFilters.Drawers
{
    [CustomGUIDrawer(typeof(IsPrefab))]
    public class IsPrefabDrawer : GUIDrawer<IsPrefab>
    {
        protected override void GUILayout(IsPrefab target)
        {
        }
    }
}
#endif