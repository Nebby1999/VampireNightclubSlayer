#if ADDRESSABLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Nebula
{
    [System.Serializable]
    public class AssetReferenceScene : AssetReference
    {
        /// <summary>
        /// Construct a new AssetReference object.
        /// </summary>
        /// <param name="guid">The guid of the asset.</param>
        public AssetReferenceScene(string guid) : base(guid)
        {
        }


        public override bool ValidateAsset(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            var type = obj.GetType();
            return typeof(UnityEditor.SceneAsset).IsAssignableFrom(type);
#else
            return false;
#endif
        }

        /// <inheritdoc/>
        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var type = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(path);
            return typeof(UnityEditor.SceneAsset).IsAssignableFrom(type);
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Type-specific override of parent editorAsset.  Used by the editor to represent the asset referenced.
        /// </summary>
        public new UnityEditor.SceneAsset editorAsset => (UnityEditor.SceneAsset)base.editorAsset;
#endif
    }

}
#endif