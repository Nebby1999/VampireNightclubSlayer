using System;
using UnityEngine;

namespace Nebula
{
    /// <summary>
    /// Base scriptable object for all of the game's scriptable objects.
    /// <br>Created to avoid having to generate unecesary data when accessing the object's .name by using <see cref="cachedName"/></br>
    /// </summary>
    public abstract class NebulaScriptableObject : ScriptableObject
    {
        private const string MESSAGE = "Retrieving the name from the engine causes allocations on runtime, use cachedName instead, if retrieving the value from the engine is absolutely necesary, cast to ScriptableObject first.";

        /// <summary>
        /// Retrieves the object's name using a cached string.
        /// </summary>
        public string cachedName
        {
            get
            {
                _cachedName ??= base.name;
                return _cachedName;
            }
            set
            {
                base.name = value;
                _cachedName = value;
            }
        }
        private string _cachedName = null;

        /// <summary>
        /// Retrieving the name from the engine causes allocations on runtime, use cachedName instead, if retrieving the value from the engine is absolutely necesary, cast to ScriptableObject first.
        /// </summary>
        [Obsolete(MESSAGE, true)]
        new public string name { get => throw new System.NotSupportedException(MESSAGE); set => throw new System.NotSupportedException(MESSAGE); }

        protected virtual void Awake()
        {
            _cachedName = base.name;
        }
    }
}