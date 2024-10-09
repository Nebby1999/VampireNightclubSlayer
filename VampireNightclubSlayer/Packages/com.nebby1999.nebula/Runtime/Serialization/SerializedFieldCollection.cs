using System;

namespace Nebula.Serialization
{
    [Serializable]
    public struct SerializedFieldCollection
    {
        public SerializedField[] serializedFields;

        public ref SerializedField GetOrCreateField(string fieldName)
        {
            if(serializedFields == null)
            {
                serializedFields = Array.Empty<SerializedField>();
            }

            for(int i = 0; i < serializedFields.Length; i++)
            {
                ref SerializedField field = ref serializedFields[i];
                if(field.fieldName.Equals(fieldName, StringComparison.Ordinal))
                {
                    return ref field;
                }
            }

            SerializedField newField = default;
            newField.fieldName = fieldName;
            ArrayUtils.Append(ref serializedFields, newField);
            ref SerializedField reference = ref serializedFields[serializedFields.Length - 1];
            return ref reference;
        }

        public void PurgeUnityPseudoNull()
        {
            if (serializedFields == null)
                serializedFields = Array.Empty<SerializedField>();

            for (int i = 0; i < serializedFields.Length; i++)
            {
                serializedFields[i].serializedValue.PurgeUnityNull();
            }
        }
    }
}