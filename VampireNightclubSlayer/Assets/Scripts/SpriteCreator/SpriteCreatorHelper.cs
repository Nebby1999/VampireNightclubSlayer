using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nebula;
using VampireSlayer.SpriteCreator;
using System;

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
        private HashSet<int> _createdSprites;

        private void Awake()
        {
            _rng = new Xoroshiro128Plus(creationSeed);
        }

        private void Start()
        {
            AssignIndices();

            StartCoroutine(Create());
        }

        private IEnumerator Create()
        {
            int i = 0;
            while(i < totalSprites)
            {
                if(_currentBody)
                {
                    Destroy(_currentBody);
                    continue;
                }
                int id = 0;

                var bodyPrefab = bodySprites.NextElementUniform(_rng);
                id += bodyPrefab.id;
                var bodyInstance = Instantiate(bodyPrefab, transform);
                _currentBody = bodyInstance.gameObject;
                bodyInstance.transform.localPosition = Vector3.zero;
                yield return new WaitForSeconds(0.1f);

                var head = headSprites.NextElementUniform(_rng);
                id += bodyPrefab.id;
                var headPivot = bodyInstance.childLocator.FindChild("Head");
                var headInstance = Instantiate(head, headPivot);
                headInstance.transform.localPosition = Vector3.zero;
                yield return new WaitForSeconds(0.1f);

                var hair = hairSprites.NextElementUniform(_rng);
                id += hair.id;
                var hairPivot = headInstance.childLocator.FindChild("Hair");
                var hairInstance = Instantiate(hair, hairPivot);
                hairInstance.transform.localPosition = Vector3.zero;
                yield return new WaitForSeconds(0.1f);

                var face = faceSprites.NextElementUniform(_rng);
                id += face.id;
                var facePivot = headInstance.childLocator.FindChild("Face");
                var faceInstance = Instantiate(face, facePivot);
                faceInstance.transform.localPosition = Vector3.zero;
                yield return new WaitForSeconds(0.1f);
                yield break;


                if (_createdSprites.Contains(id))
                {
                    continue;
                }

                _createdSprites.Add(id);
                var subroutine = SaveSprite(id);
                while(subroutine.MoveNext())
                {
                    yield return null;
                }
            }
            yield break;
        }

        private IEnumerator SaveSprite(int id)
        {
            yield break;
        }

        private void AssignIndices()
        {
            int count = 0;
            AssignInternal(headSprites, count);
            AssignInternal(faceSprites, count);
            AssignInternal(bodySprites, count);
            AssignInternal(hairSprites, count);
            AssignInternal(accesorySprites, count);

            void AssignInternal(SpriteIdentifier[] sprites, int count)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].id = count;
                    count++;
                }
            }
        }
    }
}
