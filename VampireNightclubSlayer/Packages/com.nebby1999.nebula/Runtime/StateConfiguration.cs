using EntityStates;
using Nebula.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

namespace Nebula
{
    public abstract class StateConfiguration : NebulaScriptableObject
    {
        public abstract SerializableSystemType stateTypeToConfig { get; }
        public SerializedFieldCollection fieldCollection;

        [ContextMenu("Set name to targetType name")]
        public virtual void SetNameToTargetTypeName()
        {
            Type _targetType = (Type)stateTypeToConfig;
            if (_targetType == null)
                return;

            cachedName = _targetType.FullName;
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(this), _targetType.FullName);
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Editor PlayMode-Test serialized values")]
        public virtual void RuntimeTestSerializedValues()
        {
            if (!Application.isPlaying)
                return;

            Type targetType = (Type)stateTypeToConfig;
            if (targetType == null)
                return;

            State state = InstantiateState();
            foreach (FieldInfo field in state.GetType().GetFields())
            {
                Debug.Log($"{field.Name} ({field.FieldType.Name}): {field.GetValue(state)}");
            }
        }

#endif
        public abstract State InstantiateState();

        public virtual void ApplyStaticConfiguration()
        {
            if (!Application.isPlaying)
                return;

            Type _targetType = (Type)stateTypeToConfig;
            if (_targetType == null)
            {
                Debug.LogError($"{this} has an invalid TargetType set! (targetType.TypeName value: {_targetType})");
                return;
            }

            foreach (SerializedField field in fieldCollection.serializedFields)
            {
                try
                {
                    FieldInfo fieldInfo = _targetType.GetField(field.fieldName, BindingFlags.Public | BindingFlags.Static);
                    if (fieldInfo == null)
                    {
                        continue;
                    }

                    var serializedValueForfield = field.serializedValue.GetValue(fieldInfo);
                    if (serializedValueForfield != null)
                    {
                        fieldInfo.SetValue(fieldInfo, serializedValueForfield);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public virtual Action<object> CreateInstanceInitializer()
        {
            if (!Application.isPlaying)
                return null;

            Type _targetType = (Type)stateTypeToConfig;
            if (_targetType == null)
            {
                Debug.LogError($"{this} has an invalid TargetType set! (targetType.TypeName value: {_targetType})");
                return null;
            }

            SerializedField[] serializedFields = fieldCollection.serializedFields;
            List<(FieldInfo, object)> fieldValuePair = new List<(FieldInfo, object)>();
            for(int i = 0; i < serializedFields.Length; i++)
            {
                ref SerializedField reference = ref serializedFields[i];
                try
                {
                    FieldInfo field = _targetType.GetField(reference.fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if(!(field == null) && FieldPassesFilter(field))
                    {
                        fieldValuePair.Add((field, reference.serializedValue.GetValue(field)));
                    }
                }
                catch(Exception msg)
                {
                    Debug.LogError(msg);
                }
            }

            if (fieldValuePair.Count == 0)
                return null;

            return InitializeObject;
            bool FieldPassesFilter(FieldInfo field)
            {
                return field.GetCustomAttribute<SerializeField>() != null;
            }

            void InitializeObject(object obj)
            {
                foreach (var pair in fieldValuePair)
                {
                    pair.Item1.SetValue(obj, pair.Item2);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            fieldCollection.PurgeUnityPseudoNull();
        }
    }
}