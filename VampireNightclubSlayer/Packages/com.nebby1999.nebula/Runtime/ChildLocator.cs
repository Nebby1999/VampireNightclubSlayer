using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebula
{
    public class ChildLocator : MonoBehaviour
    {
        [Serializable]
        public struct NameTransformPair
        {
            public string name;
            public Transform tiedTransform;
        }

        public NameTransformPair[] Entries => _transformPairs;
        [SerializeField]
        private NameTransformPair[] _transformPairs;

        public Transform FindChild(string name) => FindChild(FindChildIndex(name));

        public Transform FindChild(int childIndex) => _transformPairs[childIndex].tiedTransform;

        public GameObject FindChildGameObject(string name) => FindChildGameObject(FindChildIndex(name));

        public GameObject FindChildGameObject(int childIndex) => _transformPairs[childIndex].tiedTransform.gameObject;

        public T FindComponentInChild<T>(string childName) where T : Component
        {
            return FindComponentInChild<T>(FindChildIndex(childName));
        }

        public T FindComponentInChild<T>(int childIndex) where T : Component
        {
            return _transformPairs[childIndex].tiedTransform.GetComponent<T>();
        }

        public string FindChildName(int childIndex) => childIndex < _transformPairs.Length ? _transformPairs[childIndex].name : null;

        public int FindChildIndex(Transform childTransform)
        {
            for (int i = 0; i < _transformPairs.Length; i++)
            {
                if (_transformPairs[i].tiedTransform == childTransform)
                {
                    return i;
                }
            }
            return -1;
        }
        public int FindChildIndex(string name)
        {
            for (int i = 0; i < _transformPairs.Length; i++)
            {
                if (_transformPairs[i].name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}