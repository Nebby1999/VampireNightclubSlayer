using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VampireSlayer
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Person : MonoBehaviour
    {
        public PersonIndex personIndex { get; internal set; }
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
