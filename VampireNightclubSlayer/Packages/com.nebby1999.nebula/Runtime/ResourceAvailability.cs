using System;

namespace Nebula
{
    public struct ResourceAvailability
    {
        private event Action onAvailable;
        public bool isAvailable { get; private set; }

        public void CallWhenAvailable(Action method)
        {
            if(isAvailable)
            {
                method?.Invoke();
            }
            onAvailable += method;
        }

        public void MakeAvailable()
        {
            if(!isAvailable)
            {
                isAvailable = true;
                onAvailable?.Invoke();
                onAvailable = null;
            }
        }
    }
}