using System.Collections.Generic;
using System;

namespace Nebula
{
    public static class ArrayUtils
    {
        public static void InsertNoResize<T>(ref T[] array, int arraySize, int position, in T tElement)
        {
            for (int num = arraySize - 1; num > position; num--)
            {
                array[num] = array[num - 1];
            }
            array[position] = tElement;
        }

        public static void Insert<T>(ref T[] array, ref int arraySize, int position, in T tElement)
        {
            arraySize++;
            if (arraySize > array.Length)
            {
                Array.Resize(ref array, arraySize);
            }
            InsertNoResize(ref array, arraySize, position, in tElement);
        }

        public static void Insert<T>(ref T[] array, int position, in T value)
        {
            int arraySize = array.Length;
            Insert(ref array, ref arraySize, position, in value);
        }

        public static void Append<T>(ref T[] tArray, ref int arraySize, in T tElement)
        {
            Insert(ref tArray, ref arraySize, arraySize, in tElement);
        }

        public static void Append<T>(ref T[] tArray, in T tElement)
        {
            int arraySize = tArray.Length;
            Append(ref tArray, ref arraySize, tElement);
        }

        public static void RemoveAt<T>(ref T[] tArray, ref int arraySize, int pos, int count = 0)
        {
            int num = arraySize;
            arraySize -= count;
            int i = pos;
            for (int num2 = arraySize; i < num2; i++)
            {
                tArray[i] = tArray[i + count];
            }
            for (int j = arraySize; j < num; j++)
            {
                tArray[j] = default(T);
            }
        }

        public static void RemoveAtAndResize<T>(ref T[] tArray, int pos, int count)
        {
            int arraySize = tArray.Length;
            RemoveAt(ref tArray, ref arraySize, pos, count);
            Array.Resize(ref tArray, arraySize);
        }

        public static T GetSafe<T>(ref T[] tArray, int index)
        {
            if ((uint)index >= tArray.Length)
                return default(T);

            return tArray[index];
        }

        public static T GetSafe<T>(ref T[] tArray, int index, in T defaultValue)
        {
            if ((uint)index >= tArray.Length)
                return defaultValue;

            return tArray[index];
        }

        public static void SetAll<T>(ref T[] tArray, in T val)
        {
            for (int i = 0; i < tArray.Length; i++)
            {
                tArray[i] = val;
            }
        }

        public static void SetRange<T>(ref T[] tArray, in T val, int startPos, int count)
        {
            if (startPos < 0)
                throw new ArgumentOutOfRangeException("startPos", "startPos cannot be less than zero.");

            int num = startPos + count;
            if (tArray.Length < num)
                throw new ArgumentOutOfRangeException("count", "startPos + count cannot exceed tArray.Length.");

            for (int i = startPos; i < num; i++)
            {
                tArray[i] = val;
            }
        }

        public static void EnsureCapacity<T>(ref T[] tArray, int capacity)
        {
            if (tArray.Length < capacity)
            {
                Array.Resize(ref tArray, capacity);
            }
        }

        public static void Swap<T>(T[] array, int a, int b)
        {
            ref T reference = ref array[a];
            ref T reference2 = ref array[b];
            T val = reference;
            reference = reference2;
            reference2 = val;
        }

        public static void Clear<T>(T[] array, ref int count)
        {
            Array.Clear(array, 0, count);
            count = 0;
        }

        public static bool SequenceEquals<T>(T[] a, T[] b) where T : IEquatable<T>
        {
            if (a == null || b == null)
            {
                return a == null == (b == null);
            }
            if (a == b)
            {
                return true;
            }
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool SequenceEquals<T, TComparer>(T[] a, T[] b, TComparer equalityComparison) where TComparer : IEqualityComparer<T>
        {
            if (a == null || b == null)
            {
                return a == null == (b == null);
            }
            if (a == b)
            {
                return true;
            }
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (!equalityComparison.Equals(a[i], b[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static T[] Clone<T>(T[] src)
        {
            T[] array = new T[src.Length];
            Array.Copy(src, array, src.Length);
            return array;
        }

        public static TElement[] Clone<TElement, TSrc>(TSrc src) where TSrc : IReadOnlyList<TElement>
        {
            TElement[] array = new TElement[src.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = src[i];
            }
            return array;
        }

        public static void Clone<TElement, TSrc>(TSrc src, out TElement[] dest) where TSrc : IReadOnlyList<TElement>
        {
            dest = Clone<TElement, TSrc>(src);
        }

        public static void CloneTo<T>(T[] src, ref T[] dest)
        {
            Array.Resize(ref dest, src.Length);
            Array.Copy(src, dest, src.Length);
        }

        public static void CloneTo<T>(T[] src, ref T[] dest, ref int destLength)
        {
            EnsureCapacity(ref dest, src.Length);
            Array.Copy(src, dest, src.Length);
            destLength = src.Length;
        }

        public static void CloneTo<TElement, TSrc>(TSrc src, ref TElement[] dest) where TSrc : IReadOnlyList<TElement>
        {
            Array.Resize(ref dest, src.Count);
            for (int i = 0; i < dest.Length; i++)
            {
                dest[i] = src[i];
            }
        }

        public static void CloneTo<TElement, TSrc>(TSrc src, ref TElement[] dest, ref int destLength) where TSrc : IReadOnlyList<TElement>
        {
            EnsureCapacity(ref dest, src.Count);
            destLength = src.Count;
            for (int i = 0; i < destLength; i++)
            {
                dest[i] = src[i];
            }
        }

        public static bool IsInBounds<T>(T[] array, int index)
        {
            return (uint)index < array.Length;
        }

        public static bool IsInBounds<T>(T[] array, uint index)
        {
            return index < array.Length;
        }

        public static T[] Join<T>(T[] a, T[] b)
        {
            int num = a.Length + b.Length;
            if (num == 0)
            {
                return Array.Empty<T>();
            }
            T[] array = new T[num];
            Array.Copy(a, 0, array, 0, a.Length);
            Array.Copy(b, 0, array, a.Length, b.Length);
            return array;
        }

        public static T[] Join<T>(params T[][] arrays)
        {
            int num = 0;
            T[][] array = arrays;
            foreach (T[] array2 in array)
            {
                num += array2.Length;
            }
            if (num == 0)
            {
                return Array.Empty<T>();
            }
            T[] array3 = new T[num];
            int num2 = 0;
            array = arrays;
            foreach (T[] array4 in array)
            {
                Array.Copy(array4, 0, array3, num2, array4.Length);
                num2 += array4.Length;
            }
            return array3;
        }
    }
}