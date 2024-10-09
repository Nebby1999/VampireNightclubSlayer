using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

namespace Nebula
{
    public static class NebulaUtil
    {
        public static Camera mainCamera
        {
            get
            {
                if (!_mainCamera)
                    _mainCamera = Camera.main;
                return _mainCamera;
            }
        }
        private static Camera _mainCamera;

#if UNITY_EDITOR
        public static Camera sceneCamera
        {
            get
            {
                if (!_sceneCamera)
                    _sceneCamera = UnityEditor.SceneView.currentDrawingSceneView.camera;
                return _sceneCamera;
            }
        }
        private static Camera _sceneCamera;
#endif

        public static Bounds CalculateColliderBounds(GameObject obj, bool includeChildren, Func<Collider, bool> ignorePredicate = null)
        {
            Physics.SyncTransforms();
            var colliders = includeChildren ? obj.GetComponentsInChildren<Collider>(true) : obj.GetComponents<Collider>();

            var bounds = new Bounds(obj.transform.position, Vector3.one);
            if (colliders.Length == 0)
                return bounds;

            foreach (var collider in colliders)
            {
                var colliderBounds = collider.bounds;
                if (!ignorePredicate?.Invoke(collider) ?? true)
                    bounds.Encapsulate(colliderBounds);
            }
            return bounds;
        }

        public static Bounds CalculateRendererBounds(GameObject obj, bool includeChildren, Func<Renderer, bool> ignorePredicate = null)
        {
            var renderers = includeChildren ? obj.GetComponentsInChildren<Renderer>(true) : obj.GetComponents<Renderer>();

            var bounds = new Bounds(obj.transform.position, Vector3.zero);
            if (renderers.Length == 0)
                return bounds;

            foreach (var renderer in renderers)
            {
                var rendererBounds = renderer.bounds;
                if (!ignorePredicate?.Invoke(renderer) ?? true)
                    bounds.Encapsulate(rendererBounds);
            }
            return bounds;
        }

        public static Quaternion SafeLookRotation(Vector3 forward)
        {
            Quaternion result = Quaternion.identity;
            if (forward.sqrMagnitude > Mathf.Epsilon)
            {
                result = Quaternion.LookRotation(forward);
            }
            return result;
        }

        public static Quaternion SafeLookRotation(Vector3 forward, Vector3 upwards)
        {
            Quaternion result = Quaternion.identity;
            if (forward.sqrMagnitude > Mathf.Epsilon)
            {
                result = Quaternion.LookRotation(forward, upwards);
            }
            return result;
        }

        public static void DebugBounds(Bounds bounds, Color color, float duration)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector3 start = new Vector3(min.x, min.y, min.z);
            Vector3 vector = new Vector3(min.x, min.y, max.z);
            Vector3 vector2 = new Vector3(min.x, max.y, min.z);
            Vector3 end = new Vector3(min.x, max.y, max.z);
            Vector3 vector3 = new Vector3(max.x, min.y, min.z);
            Vector3 vector4 = new Vector3(max.x, min.y, max.z);
            Vector3 end2 = new Vector3(max.x, max.y, min.z);
            Vector3 start2 = new Vector3(max.x, max.y, max.z);
            Debug.DrawLine(start, vector, color, duration);
            Debug.DrawLine(start, vector3, color, duration);
            Debug.DrawLine(start, vector2, color, duration);
            Debug.DrawLine(vector2, end, color, duration);
            Debug.DrawLine(vector2, end2, color, duration);
            Debug.DrawLine(start2, end, color, duration);
            Debug.DrawLine(start2, end2, color, duration);
            Debug.DrawLine(start2, vector4, color, duration);
            Debug.DrawLine(vector4, vector3, color, duration);
            Debug.DrawLine(vector4, vector, color, duration);
            Debug.DrawLine(vector, end, color, duration);
            Debug.DrawLine(vector3, end2, color, duration);
        }
        public static void DebugCross(Vector3 position, float radius, Color color, float duration)
        {
            Debug.DrawLine(position - Vector3.right * radius, position + Vector3.right * radius, color, duration);
            Debug.DrawLine(position - Vector3.up * radius, position + Vector3.up * radius, color, duration);
            Debug.DrawLine(position - Vector3.forward * radius, position + Vector3.forward * radius, color, duration);
        }
        public static bool AnimatorParamExists(int paramHash, Animator animator)
        {
            if (!animator)
                return false;

            for (int i = 0; i < animator.parameterCount; i++)
            {
                if (animator.GetParameter(i).nameHash == paramHash)
                    return true;
            }
            return false;
        }
    }
}