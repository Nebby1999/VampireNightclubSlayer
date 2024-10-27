using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nebula
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnhancedSpriteRenderer : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer { get; private set; }
        public ShadowCastingMode shadowCastingMode;
        public bool receivesShadows;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnValidate()
        {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.shadowCastingMode = shadowCastingMode;
            renderer.receiveShadows = receivesShadows;
        }
    }
}
