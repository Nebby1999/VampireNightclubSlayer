#if SMART_ADDRESSER && ADDRESSABLES
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
using SmartAddresser.Editor.Foundation.ListableProperty;
using UnityEditor;
using UnityEngine;

namespace Nebula.Editor.SmartAddresser.AssetFilters
{
    [Serializable]
    [AssetFilter("Nebula/Non Folder Assets In Folder", "Non Folder Asets In Folder")]
    public class NonFolderAssetsInFolder : AssetFilterBase
    {
        public UnityEngine.Object FolderAsset
        {
            get => _folderAsset;
            set
            {
                var assetPath = AssetDatabase.GetAssetPath(value);
                if (string.IsNullOrEmpty(assetPath) || !AssetDatabase.IsValidFolder(assetPath))
                {
                    return;
                }
                _folderAsset = value;
            }
        }
        [SerializeField] private UnityEngine.Object _folderAsset;

        private string _parentAssetPath;
        private string[] _assetsInParentAssetPath = Array.Empty<string>();
        public override void SetupForMatching()
        {
            _parentAssetPath = AssetDatabase.GetAssetPath(_folderAsset);
            if (System.IO.Directory.Exists(_parentAssetPath))
            {
                _assetsInParentAssetPath = System.IO.Directory.EnumerateFiles(_parentAssetPath).Where(pth => !pth.EndsWith(".meta")).Select(pth => pth.Replace('\\', '/')).ToArray();
            }
        }

        public override bool IsMatch(string assetPath, Type assetType, bool isFolder)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                Debug.LogError("Unassigned ParentAsset in NonFolderAssetsInFolder Asset Filter");
                return false;
            }

            return _assetsInParentAssetPath.Contains(assetPath);
        }

        public override string GetDescription()
        {
            return "Assets in Folder: " + (FolderAsset ? FolderAsset.name : "None");
        }
    }
}
#endif