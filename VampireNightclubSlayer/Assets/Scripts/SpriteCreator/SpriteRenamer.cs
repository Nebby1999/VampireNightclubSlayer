using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VampireSlayer
{
    public class SpriteRenamer : MonoBehaviour
    {
        public TextAsset names;
        public Object[] sprites;

        [ContextMenu("DoRename")]
        private void DoRename()
        {
            Debug.Log(names.text);
            string[] namesArray = names.text.Split("\r\n");

            for (int i = 0; i < sprites.Length; i++)
            {
                Object sprite = sprites[i];
                string personName = namesArray[i];
                personName = personName.Replace(' ', ';');
#if UNITY_EDITOR
                var spriteName = sprite.name;
                var newName = personName + ";" + spriteName;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(sprite), newName);
#endif
            }
        }
    }
}
