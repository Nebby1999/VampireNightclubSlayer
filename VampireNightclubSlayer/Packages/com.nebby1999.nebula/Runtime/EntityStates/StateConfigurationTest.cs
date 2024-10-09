using System;
using UnityEngine;

namespace EntityStates
{
    public class StateConfigurationTest : State
    {
        public static LayerMask testLayerMask;
        public static char testChar;
        public static Quaternion testQuaternion;

        protected override void Initialize()
        {
        }
    }
}