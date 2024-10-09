using UnityEngine;

namespace Nebula
{
    public class ValueLabelAttribute : PropertyAttribute
    {
        public string propertyName { get; set; }
        public ValueLabelAttribute(string propertyName = null)
        {
            this.propertyName = propertyName;
        }
    }
}