using System;

namespace Nebula.Serialization
{
    [Serializable]
    public struct SerializedField
    {
        public string fieldName;
        public SerializedValue serializedValue;
    }
}