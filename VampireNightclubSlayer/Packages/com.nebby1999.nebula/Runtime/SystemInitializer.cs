using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Nebula
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class SystemInitializerAttribute : SearchableAttribute
    {
        public static bool hasExecuted { get; private set; }

        public readonly Type[] dependencies = Array.Empty<Type>();

        public MethodInfo targetMethodInfo => target as MethodInfo;

        private static Queue<SystemInitializerAttribute> _systemInitializerAttributes;
        private Type _associatedType;

        public SystemInitializerAttribute(params Type[] dependencies)
        {
            if(dependencies != null)
            {
                this.dependencies = dependencies;
            }
        }

        public static CoroutineTask Execute()
        {
            if (hasExecuted)
                return new CoroutineTask(null);

            hasExecuted = true;
            return new CoroutineTask(ExecuteInternal());
        }

        private static void EnqueueStatic()
        {
            ReadOnlyCollection<SystemInitializerAttribute> instances = SearchableAttribute.GetInstances<SystemInitializerAttribute>();
            if (instances == null)
                return;

            foreach(var attribute in instances)
            {
                if(!attribute.targetMethodInfo.IsStatic)
                {
                    continue;
                }

                attribute._associatedType = attribute.targetMethodInfo.DeclaringType;
                _systemInitializerAttributes.Enqueue(attribute);
            }
        }

        private static IEnumerator ExecuteInternal()
        {
            _systemInitializerAttributes = new Queue<SystemInitializerAttribute>();
            Thread initThread = new Thread(EnqueueStatic);
            
            initThread.Start();
            while (initThread.IsAlive)
                yield return null;


            HashSet<Type> initializedTypes = new HashSet<Type>();
            int num = 0;
            while(_systemInitializerAttributes.Count > 0)
            {
                SystemInitializerAttribute instance = _systemInitializerAttributes.Dequeue();
                if(!InitializerDependenciesMet(initializedTypes, instance))
                {
                    num++;
                    if(num >= _systemInitializerAttributes.Count)
                    {
                        Debug.LogError($"SystemInitializerAttribute: Infinite Loop Detected. (Current Method:{instance._associatedType.FullName}.{instance.targetMethodInfo.Name}())");
                        Debug.LogError($"Initializer Dependencies:\n" + string.Join('\n', (IEnumerable<Type>)instance.dependencies));
                        Debug.LogError($"Initialized Types:\n" + string.Join('\n', initializedTypes));
                        continue;
                    }
                    _systemInitializerAttributes.Enqueue(instance);
                }

                IEnumerator subroutine = null;
                try
                {
                    subroutine = instance.targetMethodInfo.Invoke(null, Array.Empty<object>()) as IEnumerator;
                }
                catch(Exception e)
                {
                    Debug.LogError($"Error during execution of SystemInitializer attached to {instance._associatedType.FullName}.{instance.targetMethodInfo.Name}().\n{e}");
                    initializedTypes.Add(instance._associatedType);
                    continue;
                }

                if(subroutine != null)
                {
                    while(subroutine.MoveNext())
                    {
                        yield return null;
                    }
                }
                initializedTypes.Add(instance._associatedType);
            }
        }

        private static bool InitializerDependenciesMet(HashSet<Type> initializedTypes, SystemInitializerAttribute attribute)
        {
            Type[] dependencies = attribute.dependencies;
            foreach (Type type in dependencies)
            {
                if (!initializedTypes.Contains(type))
                    return false;
            }
            return true;
        }
    }
}