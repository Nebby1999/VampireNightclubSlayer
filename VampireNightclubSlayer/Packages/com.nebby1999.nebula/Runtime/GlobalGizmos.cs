using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nebula
{
    public class GlobalGizmos : SingletonMonoBehaviour<GlobalGizmos>
    {
        protected override bool destroyIfDuplicate => true;
        private static List<DrawRequest> _drawRequests = new List<DrawRequest>();
        private static List<int> _finishedRequests = new List<int>();

        public static void EnqueueGizmoDrawing(Action action, int drawCalls = 60)
        {
            if(!instance)
            {
                if(Application.isPlaying)
                {
                    var go = new GameObject();
                    go.name = "DEBUG_GlobalGizmos";
                    go.AddComponent<GlobalGizmos>();
                    DontDestroyOnLoad(go);
                }
                else
                {
                    var go = new GameObject();
                    go.name = "DEBUG_GlobalGizmmos";
                    go.AddComponent<GlobalGizmos>();
                    go.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
                }
            }

            _drawRequests.Add(new DrawRequest
            {
                gizmoDrawingMethod = action,
                remainingDrawCalls = drawCalls
            });
        }

        private void OnDrawGizmos()
        {
            _finishedRequests.Clear();
            for (int i = 0; i < _drawRequests.Count; i++)
            {
                var request = _drawRequests[i];
                request.gizmoDrawingMethod();
                request.remainingDrawCalls--;
                _drawRequests[i] = request;
                if (request.remainingDrawCalls <= 0)
                {
                    _finishedRequests.Add(i);
                }
            }

            foreach (var requestIndex in _finishedRequests)
            {
                _drawRequests.RemoveAt(requestIndex);
            }
        }
        private struct DrawRequest
        {
            public Action gizmoDrawingMethod;
            public int remainingDrawCalls;
        }
    }
}