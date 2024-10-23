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
            foreach(var bodyPrefab in bodySprites)
            {
                yield return null;
                foreach(var headPrefab in headSprites)
                {
                    yield return null;
                    foreach (var hairPrefab in hairSprites)
                    {
                        yield return null;
                        foreach (var facePrefab in faceSprites)
                        {
                            yield return null;
                            int id = 0;

                            id += bodyPrefab.id;
                            var bodyInstance = Instantiate(bodyPrefab, transform);
                            _currentBody = bodyInstance.gameObject;
                            bodyInstance.transform.localPosition = Vector3.zero;

                            id += headPrefab.id;
                            var headPivot = bodyInstance.childLocator.FindChild("Head");
                            var headInstance = Instantiate(headPrefab, headPivot);
                            headInstance.transform.localPosition = Vector3.zero;

                            id += hairPrefab.id;
                            var hairPivot = headInstance.childLocator.FindChild("Hair");
                            var hairInstance = Instantiate(hairPrefab, hairPivot);
                            hairInstance.transform.localPosition = Vector3.zero;

                            id += facePrefab.id;
                            var facePivot = headInstance.childLocator.FindChild("Face");
                            var faceInstance = Instantiate(facePrefab, facePivot);
                            faceInstance.transform.localPosition = Vector3.zero;

                            yield return null;

                            Debug.Log("Saving sprite with id " + id);
                            SuccessContainer successContainer = new SuccessContainer();
                            var subroutine = SaveSprite(successContainer);
                            while (subroutine.MoveNext())
                            {
                                yield return null;
                            }
                            Destroy(_currentBody);
                            _currentBody = null;
                        }
                    }
                }
            }

            var randomSpritesSubroutine = GetRandomSpritesFromOutput();
            while(randomSpritesSubroutine.MoveNext())
            {
                yield return null;
            }

            foreach (var path in _filePaths)
            {
                if (File.Exists(path))
                {
                    AssetDatabase.ImportAsset(path);
                }
            }
        }

        private IEnumerator GetRandomSpritesFromOutput()
        {
            List<string> pathsToSave = new List<string>();
            for (int i = 0; i < totalSprites; i++)
            {
                yield return null;
                var path = _filePaths.RetrieveAndRemoveNextElementUniform(_rng);
                Debug.Log("Saving " + path);
            }

            for (int i = _filePaths.Count - 1; i >= 0; i--)
            {
                if (pathsToSave.Contains(_filePaths[i]))
                    continue;

                File.Delete(_filePaths[i]);
            }
            _filePaths = pathsToSave;
        }

        /*private IEnumerator Create()
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

                Debug.Log("Saving sprite with id " + id);
                SuccessContainer successContainer = new SuccessContainer();
                var subroutine = SaveSprite(id, successContainer);
                while(subroutine.MoveNext())
                {
                    yield return null;
                }

                if(successContainer)
                {
                    i++;
                    Debug.Log("Saved sprites count: " + i);
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
        }*/

        private IEnumerator SaveSprite(SuccessContainer container)
        {
            Camera main = NebulaUtil.mainCamera;
            var renderTexture = main.targetTexture;

            RenderTexture.active = renderTexture;

            Bounds bounds = NebulaUtil.CalculateRendererBounds(_currentBody, true);

            var minScreenPoint = main.WorldToScreenPoint(bounds.min, Camera.MonoOrStereoscopicEye.Mono);
            var maxScreenPoint = main.WorldToScreenPoint(bounds.max, Camera.MonoOrStereoscopicEye.Mono);
            var sizeScreen = main.WorldToScreenPoint(bounds.size, Camera.MonoOrStereoscopicEye.Mono);

            Texture2D tex = new Texture2D(Mathf.CeilToInt(sizeScreen.x), Mathf.CeilToInt(sizeScreen.y), TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            RenderTexture.active = null;

            var bytes = tex.EncodeToPNG();

            var outputDirectory = $"{folderOutput}\\SpriteCreatorOutput";
            if(!Directory.Exists(outputDirectory))
            {
                yield return null;
                Directory.CreateDirectory(outputDirectory);
            }

            var filePath = $"{outputDirectory}\\{Time.frameCount}.png";
            var task = File.WriteAllBytesAsync(filePath, bytes);
            while(!task.IsCompleted)
            {
                yield return null;
            }
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
