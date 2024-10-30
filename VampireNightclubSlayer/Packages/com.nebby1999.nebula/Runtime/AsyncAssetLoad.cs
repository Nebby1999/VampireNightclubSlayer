using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nebula
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AsyncAssetLoadAttribute : SearchableAttribute
    {
        public MethodInfo targetMethodInfo => target as MethodInfo;

        public static ParallelCoroutineTask CreateParallelCoroutineTaskForAssembly(Assembly callingAsembly)
        {
            var instances = GetInstances<AsyncAssetLoadAttribute>();
            if(instances == null)
            {
                return new ParallelCoroutineTask();
            }

            AsyncAssetLoadAttribute[] attributesForAssembly = instances.Where(IsValid).Where(att => IsFromAssembly(att, callingAsembly)).ToArray();

            var coroutineTask = new ParallelCoroutineTask();
            foreach(var att in attributesForAssembly)
            {
                coroutineTask.Add((IEnumerator)att.targetMethodInfo.Invoke(null, null));
            }
            return coroutineTask;
        }

        private static bool IsValid(AsyncAssetLoadAttribute attribute)
        {
            var methodInfo = attribute.targetMethodInfo;
            if (!methodInfo.IsStatic)
                return false;

            var returnType = methodInfo.ReturnType;
            if (returnType == null || returnType == typeof(void))
                return false;

            if (!returnType.IsSameOrSubclassOf(typeof(IEnumerator)))
                return false;

            if (methodInfo.IsGenericMethod)
                return false;

            var parameters = methodInfo.GetParameters();
            if(parameters.Length != 0)
                return false;

            return true;
        }

        private static bool IsFromAssembly(AsyncAssetLoadAttribute attribute, Assembly assembly)
        {
            var methodInfo = attribute.targetMethodInfo;

            var declaringType = methodInfo.DeclaringType;
            return declaringType.Assembly == assembly;
        }
    }
}