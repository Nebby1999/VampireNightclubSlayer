using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Nebula
{
    public static class Extensions
    {
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text));
        }

        public static string WithAllWhitespaceStripped(this string str)
        {
            var buffer = new StringBuilder();
            foreach (var ch in str)
                if (!char.IsWhiteSpace(ch))
                    buffer.Append(ch);
            return buffer.ToString();
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static ref T NextElementUniform<T>(this T[] array, Xoroshiro128Plus rng)
        {
            return ref rng.NextElementUniform(array);
        }

        public static T NextElementUniform<T>(this List<T> list, Xoroshiro128Plus rng)
        {
            return rng.NextElementUniform(list);
        }

        public static T NextElementUniform<T>(this IList<T> list, Xoroshiro128Plus rng)
        {
            return rng.NextElementUniform(list);
        }

        public static T RetrieveAndRemoveNextElementUniform<T>(this T[] array, Xoroshiro128Plus rng)
        {
            return rng.RetrieveAndRemoveNextElementUniform(array);
        }

        public static T RetrieveAndRemoveNextElementUniform<T>(this List<T> list, Xoroshiro128Plus rng)
        {
            return rng.RetrieveAndRemoveNextElementUniform(list);
        }

        public static T RetrieveAndRemoveNextElementUniform<T>(this IList<T> list, Xoroshiro128Plus rng)
        {
            return rng.RetrieveAndRemoveNextElementUniform(list);
        }

        public static T AsValidOrNull<T>(this T t) where T : UnityEngine.Object => t ? t : null;

        public static T EnsureComponent<T>(this Component c) where T : Component => EnsureComponent<T>(c.gameObject);
        public static T EnsureComponent<T>(this Behaviour b) where T : Component => EnsureComponent<T>(b.gameObject);
        public static T EnsureComponent<T>(this MonoBehaviour mb) where T : Component => EnsureComponent<T>(mb.gameObject);
        public static T EnsureComponent<T>(this GameObject go) where T : Component => go.GetComponent<T>().AsValidOrNull() ?? go.AddComponent<T>();

        public static bool CompareLayer(this Component c, int layerIndex) => CompareLayer(c.gameObject, layerIndex);
        public static bool CompareLayer(this Behaviour b, int layerIndex) => CompareLayer(b.gameObject, layerIndex);
        public static bool CompareLayer(this MonoBehaviour mb, int layerIndex) => CompareLayer(mb.gameObject, layerIndex);
        public static bool CompareLayer(this GameObject go, int layerIndex) => go.layer == layerIndex;

        public static bool Contains(this LayerMask mask, int layerIndex)
        {
            return mask == (mask | (1 << layerIndex));
        }

        public static Color32 GetBestOutline(this Color32 color32)
        {
            Color asColor = (Color)color32;
            return (Color32)asColor.GetBestOutline();
        }

        public static Color GetBestOutline(this Color color)
        {
            Color.RGBToHSV(color, out float hue, out float saturation, out float brightness);

            float modifier = brightness > 0.5f ? (-0.5f) : 0.5f;
            float newSaturation = Mathf.Clamp01(saturation + modifier);
            float newBrightness = Mathf.Clamp01(brightness + modifier);
            return Color.HSVToRGB(hue, newSaturation, newBrightness);
        }
    }
}