using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace VampireSlayer
{
    public class GameManager : MonoBehaviour
    {
        [Header("Spline Settings")]
        public UnityEngine.Splines.SplineContainer spline;

        public int count;
        private void Start()
        {
            spline.Warmup();
            float tForEachCount = 1f / count;
            for(int i = 0; i <  count; i++)
            {
                spline.Evaluate(tForEachCount + (tForEachCount * i), out float3 position, out float3 tangent, out float3 upVector);
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.name = i.ToString();
                obj.transform.position = position;
                obj.transform.forward = tangent;
            }
        }
    }
}