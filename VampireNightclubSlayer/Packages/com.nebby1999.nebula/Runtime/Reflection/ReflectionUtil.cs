using System;
using System.Linq;
using System.Reflection;

namespace Nebula
{
    public static class ReflectionUtil
    {
        public static readonly BindingFlags all = (BindingFlags)(-1);

        public static Type[] GetTypesSafe(this Assembly assembly)
        {
            Type[] types = null;
            try
            {
                types = assembly.GetTypes();
            }
            catch(ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }
            return types;
        }

       public static bool IsSameOrSubclassOf(this Type type, Type otherType)
        {
            if (!(type == otherType))
            {
                return type.IsSubclassOf(otherType);
            }

            return true;
        }
    }
}