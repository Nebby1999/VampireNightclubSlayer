using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using Random = UnityEngine.Random;

namespace Nebula
{
    public class Xoroshiro128Plus
    {
        public class SplitMix64
        {
            public ulong x;

            public ulong Next()
            {
                long num1 = (long)(this.x += 11400714819323198485UL);
                long num2 = (num1 ^ (long)((ulong)num1 >> 30)) * -4658895280553007687L;
                long num3 = (num2 ^ (long)((ulong)num2 >> 27)) * -7723592293110705685L;
                return (ulong)num3 ^ (ulong)num3 >> 31;
            }
        }
        private ulong state0;
        private ulong state1;
        private static readonly SplitMix64 initializer = new SplitMix64();
        private const ulong JUMP0 = 16109378705422636197;
        private const ulong JUMP1 = 1659688472399708668;
        private static readonly ulong[] JUMP = new ulong[2]
        {
    16109378705422636197UL,
    1659688472399708668UL
        };
        private const ulong LONG_JUMP0 = 15179817016004374139;
        private const ulong LONG_JUMP1 = 15987667697637423809;
        private static readonly ulong[] LONG_JUMP = new ulong[2]
        {
    15179817016004374139UL,
    15987667697637423809UL
        };
        private static readonly int[] logTable256 = Xoroshiro128Plus.GenerateLogTable();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft(ulong x, int k) => x << k | x >> 64 - k;

        public Xoroshiro128Plus(ulong seed) => this.ResetSeed(seed);

        public Xoroshiro128Plus(Xoroshiro128Plus other)
        {
            this.state0 = other.state0;
            this.state1 = other.state1;
        }

        public void ResetSeed(ulong seed)
        {
            Xoroshiro128Plus.initializer.x = seed;
            this.state0 = Xoroshiro128Plus.initializer.Next();
            this.state1 = Xoroshiro128Plus.initializer.Next();
        }

        public ulong Next()
        {
            ulong state0 = this.state0;
            ulong state1 = this.state1;
            long num = (long)state0 + (long)state1;
            ulong x = state1 ^ state0;
            this.state0 = (ulong)((long)Xoroshiro128Plus.RotateLeft(state0, 24) ^ (long)x ^ (long)x << 16);
            this.state1 = Xoroshiro128Plus.RotateLeft(x, 37);
            return (ulong)num;
        }

        public void Jump()
        {
            ulong num1 = 0;
            ulong num2 = 0;
            for (int index1 = 0; index1 < Xoroshiro128Plus.JUMP.Length; ++index1)
            {
                for (int index2 = 0; index2 < 64; ++index2)
                {
                    if (((long)Xoroshiro128Plus.JUMP[index1] & 1L) << index2 != 0L)
                    {
                        num1 ^= this.state0;
                        num2 ^= this.state1;
                    }
                    long num3 = (long)this.Next();
                }
            }
            this.state0 = num1;
            this.state1 = num2;
        }

        public void LongJump()
        {
            ulong num1 = 0;
            ulong num2 = 0;
            for (int index1 = 0; index1 < Xoroshiro128Plus.LONG_JUMP.Length; ++index1)
            {
                for (int index2 = 0; index2 < 64; ++index2)
                {
                    if (((long)Xoroshiro128Plus.LONG_JUMP[index1] & 1L) << index2 != 0L)
                    {
                        num1 ^= this.state0;
                        num2 ^= this.state1;
                    }
                    long num3 = (long)this.Next();
                }
            }
            this.state0 = num1;
            this.state1 = num2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ToDouble01Fast(ulong x) => BitConverter.Int64BitsToDouble(4607182418800017408L | (long)(x >> 12));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ToDouble01(ulong x) => (double)(x >> 11) * 1.11022302462516E-16;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ToFloat01(uint x) => (float)(x >> 8) * 5.960464E-08f;

        public bool nextBool => this.nextLong < 0L;

        public uint nextUInt => (uint)this.Next();

        public int nextInt => (int)this.Next();

        public ulong nextULong => this.Next();

        public long nextLong => (long)this.Next();

        public double nextNormalizedDouble => Xoroshiro128Plus.ToDouble01Fast(this.Next());

        public float nextNormalizedFloat => Xoroshiro128Plus.ToFloat01(this.nextUInt);

        public float RangeFloat(float minInclusive, float maxInclusive) => minInclusive + (maxInclusive - minInclusive) * this.nextNormalizedFloat;

        public int RangeInt(int minInclusive, int maxExclusive) => minInclusive + (int)this.RangeUInt32Uniform((uint)(maxExclusive - minInclusive));

        public long RangeLong(long minInclusive, long maxExclusive) => minInclusive + (long)this.RangeUInt64Uniform((ulong)(maxExclusive - minInclusive));

        private ulong RangeUInt64Uniform(ulong maxExclusive)
        {
            if (maxExclusive == 0UL)
                throw new ArgumentOutOfRangeException("Range cannot have size of zero.");
            int num1 = 64 - Xoroshiro128Plus.CalcRequiredBits(maxExclusive);
            ulong num2;
            do
            {
                num2 = this.nextULong >> num1;
            }
            while (num2 >= maxExclusive);
            return num2;
        }

        private uint RangeUInt32Uniform(uint maxExclusive)
        {
            if (maxExclusive == 0U)
                throw new ArgumentOutOfRangeException("Range cannot have size of zero.");
            int num1 = 32 - Xoroshiro128Plus.CalcRequiredBits(maxExclusive);
            uint num2;
            do
            {
                num2 = this.nextUInt >> num1;
            }
            while (num2 >= maxExclusive);
            return num2;
        }

        private static int[] GenerateLogTable()
        {
            int[] logTable = new int[256];
            logTable[0] = logTable[1] = 0;
            for (int index = 2; index < 256; ++index)
                logTable[index] = 1 + logTable[index / 2];
            logTable[0] = -1;
            return logTable;
        }

        private static int CalcRequiredBits(ulong v)
        {
            int num = 0;
            while (v != 0UL)
            {
                v >>= 1;
                ++num;
            }
            return num;
        }

        private static int CalcRequiredBits(uint v)
        {
            int num = 0;
            while (v != 0U)
            {
                v >>= 1;
                ++num;
            }
            return num;
        }

        public Vector2 InsideUnitCircle()
        {
            var previousState = Random.state;
            Random.InitState(nextInt);
            Vector2 result = Random.insideUnitCircle;
            Random.state = previousState;
            return result;
        }

        public Vector3 InsideUnitSphere()
        {
            var previousState = Random.state;
            Random.InitState(nextInt);
            Vector3 result = Random.insideUnitSphere;
            Random.state = previousState;
            return result;
        }

        public Vector3 OnUnitSphere()
        {
            var previousState = Random.state;
            Random.InitState(nextInt);
            Vector3 result = Random.onUnitSphere;
            Random.state = previousState;
            return result;
        }

        public ref T NextElementUniform<T>(T[] array) => ref array[this.RangeInt(0, array.Length)];

        public T NextElementUniform<T>(List<T> list) => list[this.RangeInt(0, list.Count)];

        public T NextElementUniform<T>(IList<T> list) => list[this.RangeInt(0, list.Count)];

        public T RetrieveAndRemoveNextElementUniform<T>(ref T[] array)
        {
            int index = RangeInt(0, array.Length);
            T element = array[index];
            ArrayUtils.RemoveAtAndResize(ref array, index, 1);
            return element;
        }

        public T RetrieveAndRemoveNextElementUniform<T>(List<T> list)
        {
            int index = RangeInt(0, list.Count);
            T element = list[index];
            list.RemoveAt(index);
            return element;
        }

        public T RetrieveAndRemoveNextElementUniform<T>(IList<T> list)
        {
            int index = RangeInt(0, list.Count);
            T element = list[index];
            list.RemoveAt(index);
            return element;
        }
    }
}