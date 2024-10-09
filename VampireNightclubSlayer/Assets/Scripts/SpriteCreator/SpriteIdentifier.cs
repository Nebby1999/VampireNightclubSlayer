using System;
using Nebula;
using UnityEngine;

namespace VampireSlayer.SpriteCreator
{
    [RequireComponent(typeof(ChildLocator))]
    public class SpriteIdentifier : MonoBehaviour
    {
        public SpriteType type;

        public ChildLocator childLocator { get; private set; }
        public SpriteRenderer spriteRenderer { get; private set; }

        [NonSerialized]
        public int id;

        private void Awake()
        {
            childLocator = GetComponent<ChildLocator>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }
}