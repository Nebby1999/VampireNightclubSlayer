using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nebula
{
    public class SearchableAttribute : Attribute
    {
        [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
        public class OptInAttribute : Attribute
        {

        }

        private static Dictionary<Type, List<SearchableAttribute>> _attributeTypeToInstances = new Dictionary<Type, List<SearchableAttribute>>();
        private static bool _initialized;

        public object target { get; private set; }

        public static async Task Init()
        {
            if (_initialized)
                return;

            _initialized = true;
            Assembly[] assemblies = GetOptedInAssemblies();
            foreach(var assembly in assemblies)
            {
                await ScanAssembly(assembly);
            }
        }

        public static ReadOnlyCollection<T>? GetInstances<T>() where T : SearchableAttribute
        {
            if(_attributeTypeToInstances.TryGetValue(typeof(T), out var instances))
            {
                return new ReadOnlyCollection<T>(instances.Cast<T>().ToList());
            }
            return null;
        }

        public static bool TryGetInstances<T>(out ReadOnlyCollection<T> result) where T : SearchableAttribute
        {
            if(_attributeTypeToInstances.TryGetValue(typeof(T), out var list))
            {
                result = new ReadOnlyCollection<T>(list.Cast<T>().ToList());
                return true;
            }
            result = null;
            return false;
        }

        private static async Task ScanAssembly(Assembly assembly)
        {
            Type[] typesInAssembly = assembly.GetTypesSafe();
            foreach(var type in typesInAssembly)
            {
                await ScanType(type);
            }
        }

        private static async Task ScanType(Type type)
        {
            foreach (var searchableAttribute in type.GetCustomAttributes<SearchableAttribute>(false))
            {
                await RegisterAttribute(searchableAttribute, type);
            }

            var memberInfos = type.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach(var memberInfo in memberInfos)
            {
                //Scan nested types
                if(memberInfo.MemberType.HasFlag(MemberTypes.TypeInfo) || memberInfo.MemberType.HasFlag(MemberTypes.NestedType))
                {
                    await ScanType(memberInfo as Type);
                }

                foreach(var searchableAttribute in memberInfo.GetCustomAttributes<SearchableAttribute>(false))
                {
                    await RegisterAttribute(searchableAttribute, memberInfo);
                }
            }
        }

        private static Task RegisterAttribute(SearchableAttribute instance, object target)
        {
            instance.target = target;
            Type type = instance.GetType();
            while (type != null && typeof(SearchableAttribute).IsAssignableFrom(type))
            {
                GetInstancesListForType(type).Add(instance);
                type = type.BaseType;
            }
            return Task.CompletedTask;
        }

        private static List<SearchableAttribute> GetInstancesListForType(Type type)
        {
            if(!_attributeTypeToInstances.TryGetValue(type, out List<SearchableAttribute> instances))
            {
                instances = new List<SearchableAttribute>();
                _attributeTypeToInstances[type] = instances;
                return instances;
            }
            return instances;
        }

        private static Assembly[] GetOptedInAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetCustomAttribute<OptInAttribute>() != null)
                .ToArray();
            return assemblies;
        }
    }
}