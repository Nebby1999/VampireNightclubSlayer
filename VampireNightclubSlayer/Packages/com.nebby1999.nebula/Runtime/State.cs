using Nebula;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EntityStates
{
    public abstract class State
    {
        public readonly string stateName;
        public readonly string fullStateName;
        public StateMachine outer;
        public GameObject gameObject => outer.gameObject;
        public Transform transform => outer.transform;
        public float fixedAge { get; protected set; }
        public float age { get; protected set; }
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void FixedUpdate()
        {
            fixedAge += Time.fixedDeltaTime;
        }
        public virtual void Update()
        {
            age += Time.deltaTime;
        }
        public virtual void ModifyNextState(State nextState) { }
        protected void Destroy(UnityEngine.Object obj) => UnityEngine.Object.Destroy(obj);
        protected T Instantiate<T>(T obj) where T : UnityEngine.Object => UnityEngine.Object.Instantiate(obj);
        protected T Instantiate<T>(T obj, Transform parent) where T : UnityEngine.Object => UnityEngine.Object.Instantiate(obj, parent);
        protected T Instantiate<T>(T obj, Vector3 position, Quaternion rotation) where T : UnityEngine.Object => UnityEngine.Object.Instantiate(obj, position, rotation);
        protected T Instantiate<T>(T obj, Vector3 position, Quaternion rotation, Transform parent) where T : UnityEngine.Object => UnityEngine.Object.Instantiate(obj, position, rotation, parent);

        protected T GetComponent<T>() => outer.GetComponent<T>();

        protected bool TryGetComponent<T>(out T component)
        {
            return outer.TryGetComponent(out component);
        }

        protected bool TryGetComponent(Type componentType, out Component component)
        {
            return outer.TryGetComponent(componentType, out component);
        }

        protected Component GetComponent(Type type) => outer.GetComponent(type);

        protected Component GetComponent(string name) => outer.GetComponent(name);

        protected virtual Animator GetAnimator()
        {
            return null;
        }


        protected void PlayAnimation(string layerName, string animationStateName, string playbackRateParam, float duration)
        {
            if (duration <= 0f)
            {
                LogWarning("EntityState.PlayAnimation: Zero duration is not allowed.");
                return;
            }
            Animator modelAnimator = GetAnimator();
            if ((bool)modelAnimator)
            {
                PlayAnimationOnAnimator(modelAnimator, layerName, animationStateName, playbackRateParam, duration);
            }
        }

        protected static void PlayAnimationOnAnimator(Animator modelAnimator, string layerName, string animationStateName, string playbackRateParam, float duration)
        {
            modelAnimator.speed = 1f;
            modelAnimator.Update(0f);
            int layerIndex = modelAnimator.GetLayerIndex(layerName);
            if (layerIndex >= 0)
            {
                modelAnimator.SetFloat(playbackRateParam, 1f);
                modelAnimator.PlayInFixedTime(animationStateName, layerIndex, 0f);
                modelAnimator.Update(0f);
                float length = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
                modelAnimator.SetFloat(playbackRateParam, length / duration);
            }
        }

        protected void PlayCrossfade(string layerName, string animationStateName, string playbackRateParam, float duration, float crossfadeDuration)
        {
            if (duration <= 0f)
            {
                LogWarning("EntityState.PlayCrossfade: Zero duration is not allowed.");
                return;
            }
            Animator modelAnimator = GetAnimator();
            if ((bool)modelAnimator)
            {
                modelAnimator.speed = 1f;
                modelAnimator.Update(0f);
                int layerIndex = modelAnimator.GetLayerIndex(layerName);
                modelAnimator.SetFloat(playbackRateParam, 1f);
                modelAnimator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
                modelAnimator.Update(0f);
                float length = modelAnimator.GetNextAnimatorStateInfo(layerIndex).length;
                modelAnimator.SetFloat(playbackRateParam, length / duration);
            }
        }

        protected void PlayCrossfade(string layerName, string animationStateName, float crossfadeDuration)
        {
            Animator modelAnimator = GetAnimator();
            if ((bool)modelAnimator)
            {
                modelAnimator.speed = 1f;
                modelAnimator.Update(0f);
                int layerIndex = modelAnimator.GetLayerIndex(layerName);
                modelAnimator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
            }
        }

        public virtual void PlayAnimation(string layerName, string animationStateName)
        {
            Animator modelAnimator = GetAnimator();
            if ((bool)modelAnimator)
            {
                PlayAnimationOnAnimator(modelAnimator, layerName, animationStateName);
            }
        }

        protected static void PlayAnimationOnAnimator(Animator modelAnimator, string layerName, string animationStateName)
        {
            int layerIndex = modelAnimator.GetLayerIndex(layerName);
            modelAnimator.speed = 1f;
            modelAnimator.Update(0f);
            modelAnimator.PlayInFixedTime(animationStateName, layerIndex, 0f);
        }

        protected void Log(object message, [CallerMemberName] string memberName = "")
        {
            Debug.Log($"[{stateName}.{memberName}]: {message} (Type=\"{fullStateName}\")");
        }

        protected void LogWarning(object message, [CallerMemberName] string memberName = "")
        {
            Debug.LogWarning($"[{stateName}.{memberName}]: {message} (Type=\"{fullStateName}\")");
        }

        protected void LogError(object message, [CallerMemberName] string memberName = "")
        {
            Debug.LogError($"[{stateName}.{memberName}]: {message} (Type=\"{fullStateName}\")");
        }

        protected abstract void Initialize();

        public State()
        {
            Initialize();
            Type type = GetType();
            stateName = type.Name;
            fullStateName = type.FullName;
        }
    }
}