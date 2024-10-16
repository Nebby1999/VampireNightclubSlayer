using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nebula;
using VampireSlayer.SpriteCreator;
using System;
using System.IO;
using UnityEditor;

namespace VampireSlayer
{
    public class SpriteCreatorHelper : MonoBehaviour
    {
        public string folderOutput = "Assets";
        public int totalSprites = 50;
        public SpriteIdentifier[] headSprites = Array.Empty<SpriteIdentifier>();
        public SpriteIdentifier[] faceSprites = Array.Empty<SpriteIdentifier>();
        public SpriteIdentifier[] bodySprites = Array.Empty<SpriteIdentifier>();
        public SpriteIdentifier[] hairSprites = Array.Empty<SpriteIdentifier>();
        public SpriteIdentifier[] accesorySprites = Array.Empty<SpriteIdentifier>();
        public ulong creationSeed = 131999200744330U;

        private Xoroshiro128Plus _rng;
        private GameObject _currentBody;
        private List<int> _createdSprites = new List<int>();
        private List<string> _filePaths = new List<string>();

        private void Awake()
        {
            _rng = new Xoroshiro128Plus(creationSeed);
        }

        private void Start()
        {
            AssignIndices();
#if UNITY_EDITOR
            AssetDatabase.StartAssetEditing();
#endif
            StartCoroutine(Create());
        }

        private IEnumerator Create()
        {
            int i = 0;
            while(i < totalSprites)
            {
                if(_currentBody)
                {
                    DestroyImmediate(_currentBody);
                    continue;
                }
                int id = 0;

                var bodyPrefab = bodySprites.NextElementUniform(_rng);
                id += bodyPrefab.id;
                var bodyInstance = Instantiate(bodyPrefab, transform);
                _currentBody = bodyInstance.gameObject;
                bodyInstance.transform.localPosition = Vector3.zero;
                yield return null;

                var headPrefab = headSprites.NextElementUniform(_rng);
                id += headPrefab.id;
                var headPivot = bodyInstance.childLocator.FindChild("Head");
                var headInstance = Instantiate(headPrefab, headPivot);
                headInstance.transform.localPosition = Vector3.zero;
                yield return null;

                var hairPrefab = hairSprites.NextElementUniform(_rng);
                id += hairPrefab.id;
                var hairPivot = headInstance.childLocator.FindChild("Hair");
                var hairInstance = Instantiate(hairPrefab, hairPivot);
                hairInstance.transform.localPosition = Vector3.zero;
                yield return null;

                var facePrefab = faceSprites.NextElementUniform(_rng);
                id += facePrefab.id;
                var facePivot = headInstance.childLocator.FindChild("Face");
                var faceInstance = Instantiate(facePrefab, facePivot);
                faceInstance.transform.localPosition = Vector3.zero;
                yield return null;

                var accesoryPrefab = accesorySprites.NextElementUniform(_rng);
                id += accesoryPrefab.id;
                var accesoryInstance = Instantiate(accesoryPrefab, facePivot);
                accesoryInstance.transform.localPosition = Vector3.zero;
                yield return null;

                if (_createdSprites.Contains(id))
                {
                    continue;
                }

                SuccessContainer successContainer = new SuccessContainer();
                var subroutine = SaveSprite(id, successContainer);
                while(subroutine.MoveNext())
                {
                    yield return null;
                }

                if(successContainer)
                {
                    i++;
                }
            }

            foreach(var path in _filePaths)
            {
                if(File.Exists(path))
                {
                    AssetDatabase.ImportAsset(path);
                }
            }
            yield break;
        }

        private IEnumerator SaveSprite(int id, SuccessContainer container)
        {
            Camera main = NebulaUtil.mainCamera;
            var renderTexture = main.targetTexture;

            RenderTexture.active = renderTexture;
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            RenderTexture.active = null;

            var bytes = tex.EncodeToPNG();

            var outputDirectory = $"{folderOutput}\\SpriteCreatorOutput";
            if(!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var filePath = $"{outputDirectory}\\{id}.png";
            var task = File.WriteAllBytesAsync(filePath, bytes);
            while(!task.IsCompleted)
            {
                yield return null;
            }
            _createdSprites.Add(id);
            _filePaths.Add(filePath);
            container.success = true;
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            AssetDatabase.StopAssetEditing();
#endif
        }
        private void AssignIndices()
        {
            int count = 0;
            AssignInternal(headSprites);
            AssignInternal(faceSprites);
            AssignInternal(bodySprites);
            AssignInternal(hairSprites);
            AssignInternal(accesorySprites);

            void AssignInternal(SpriteIdentifier[] sprites)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].id = count;
                    count++;
                    Debug.LogWarning($"{sprites[i]}'s ID: {sprites[i].id}");
                }
            }
        }

        private class SuccessContainer
        {
            public bool success;

            public static implicit operator bool(SuccessContainer c) => c.success;
        }
    }
}
