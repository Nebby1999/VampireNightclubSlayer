using System;
using UnityEngine;

namespace Nebula
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisabledFieldAttribute : PropertyAttribute
    {

    }
}