#if USE_ADDRESSABLES && USE_SMART_ADDRESSER
using SmartAddresser.Editor.Foundation.CustomDrawers;
using UnityEditor;

namespace Nebula.Editor.SmartAddresser.AddressProviders.Drawers
{
    [CustomGUIDrawer(typeof(SimplifiedAssetPath))]
    public class SimplifiedAssetPathDrawer : GUIDrawer<SimplifiedAssetPath>
    {
        protected override void GUILayout(SimplifiedAssetPath target)
        {
            EditorGUILayout.LabelField("Simplified Asset Path");
        }
    }
}
#endif