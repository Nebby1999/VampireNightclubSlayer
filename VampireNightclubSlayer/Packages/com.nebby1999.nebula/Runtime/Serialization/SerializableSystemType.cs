using System;
using UnityEngine;

namespace Nebula.Serialization
{
    [Serializable]
    public struct SerializableSystemType
    {
        [SerializeField]
        private string _typeName;

        [Obsolete("This obtains the Type stored in this SerializableSystemType, if you want the actual Type of SerializableSystemType, use \"typeof(SerializableSystemType)\"")]
        public new Type GetType()
        {
            return Type.GetType(_typeName);
        }

        public static explicit operator Type(SerializableSystemType type)
        {
    #pragma warning disable CS0618 // Type or member is obsolete
            return type.GetType();
    #pragma warning restore CS0618 // Type or member is obsolete
        }
        public SerializableSystemType(string typeName) => _typeName = typeName;
        public SerializableSystemType(Type type) => _typeName = type.AssemblyQualifiedName;
    
        public class RequiredBaseTypeAttribute : PropertyAttribute
        {
            public Type type { get; private set; }
            public RequiredBaseTypeAttribute(Type baseType)
            {
                type = baseType;
            }
            private RequiredBaseTypeAttribute()
            {

            }
        }
    }
}