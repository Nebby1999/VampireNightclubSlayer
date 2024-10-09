using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebula
{
    public static class InstanceTracker
    {
        private static class TypeData<T> where T : MonoBehaviour
        {
            public static readonly List<T> instances;

            static TypeData()
            {
                instances = new List<T>();
            }

            public static int Add(T instance)
            {
                instances.Add(instance);
                return instances.Count;
            }

            public static int Remove(T instance)
            {
                instances.Remove(instance);
                return instances.Count;
            }
        }

        public static int Add<T>(T instance) where T : MonoBehaviour
        {
            return TypeData<T>.Add(instance);
        }

        public static int Remove<T>(T instance) where T : MonoBehaviour
        {
            return TypeData<T>.Remove(instance);
        }

        public static List<T> GetInstances<T>() where T : MonoBehaviour
        {
            return TypeData<T>.instances;
        }

        public static bool Any<T>() where T : MonoBehaviour
        {
            return TypeData<T>.instances.Count > 0;
        }

        public static T FirstOrDefault<T>() where T : MonoBehaviour
        {
            if (TypeData<T>.instances.Count == 0)
            {
                return null;
            }
            return TypeData<T>.instances[0];
        }

        public static T Random<T>(Xoroshiro128Plus rng = null) where T : MonoBehaviour
        {
            if (TypeData<T>.instances.Count == 0)
                return null;

            rng ??= new Xoroshiro128Plus((ulong)UnityEngine.Random.seed);
            return TypeData<T>.instances[rng.RangeInt(0, TypeData<T>.instances.Count)];
        }
    }
}