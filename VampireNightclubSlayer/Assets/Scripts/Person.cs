using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VampireSlayer
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Person : MonoBehaviour
    {
        [field:SerializeField]
        [field:HideInInspector]
        public string personName { get; internal set; }

        [field: SerializeField]
        [field: HideInInspector]
        public int personUUID { get; internal set; }

        [field: SerializeField]
        [field: HideInInspector]
        public PersonIndex personIndex { get; internal set; }

        [field: SerializeField]
        [field: HideInInspector]
        public Sprite personSprite { get; internal set; }

        public SpriteRenderer spriteRenderer { get; private set; }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            spriteRenderer.sprite = personSprite;
        }
    }
}
