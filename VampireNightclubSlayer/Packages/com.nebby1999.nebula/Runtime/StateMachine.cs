using EntityStates;
using Nebula.Serialization;
using System;
using UnityEngine;

namespace Nebula
{
    /// <summary>
    /// A StateMachine is a custom monobehaviour class that allows you to run classes inheriting from <see cref="State"/>, these classes allows you to have different machines controlling different parts of a GameObject.
    /// </summary>
    public abstract class StateMachine : MonoBehaviour
    {
        /// <summary>
        /// The name for this state machine
        /// </summary>
        public string stateMachineName;
        /// <summary>
        /// When the object is first instantiated, this is it's initial state.
        /// </summary>
        public abstract SerializableSystemType initialStateType { get; }
        /// <summary>
        /// Whenever a state uses SetNextStateToMain, the machine's state is set to this state.
        /// </summary>
        public abstract SerializableSystemType mainStateType { get; }

        /// <summary>
        /// The new incoming state to change to
        /// </summary>
        public State newState { get; private set; }

        /// <summary>
        /// The current state that's running
        /// </summary>
        public State currentState
        {
            get => _currentState;
            private set
            {
                if (value == null)
                    return;

                _currentState?.ModifyNextState(value);
                _currentState?.OnExit();
                _currentState = value;
                _currentState.OnEnter();
            }
        }
        private State _currentState;

        /// <summary>
        /// A unique ID set for this state machine
        /// </summary>
        public int stateMachineID { get; private set; }

        /// <summary>
        /// Initializes a new State class from the specified <see cref="Type"/>
        /// </summary>
        /// <param name="stateType">The state type that'll be initialized</param>
        /// <returns>The instance of the state</returns>
        protected abstract State InitializeState(Type stateType);

        /// <summary>
        /// Assigns the <see cref="stateMachineID"/> and <see cref="currentState"/> to <see cref="Uninitialized"/>
        /// </summary>
        protected virtual void Awake()
        {
            stateMachineID = stateMachineName.GetHashCode();
            currentState = new Uninitialized();
            currentState.outer = this;
        }

        /// <summary>
        /// Sets the current state to <see cref="initialStateType"/> IF <see cref="currentState"/> is <see cref="Uninitialized"/>
        /// </summary>
        protected virtual void Start()
        {
            var initState = (Type)initialStateType;
            if(currentState is Uninitialized && initState != null)
            {
                SetState(InitializeState(initState));
            }
        }

        /// <summary>
        /// Sets a new state for this StateMachine.
        /// </summary>
        /// <param name="newState">The new state for this state machine</param>
        /// <exception cref="NullReferenceException"><paramref name="newState"/> is null</exception>
        protected virtual void SetState(State newState)
        {
            if (newState == null)
                throw new NullReferenceException("newState is null");

            newState.outer = this;
            this.newState = null;
            currentState = newState;
        }

        /// <summary>
        /// Sets a new state for this StateMachine.
        /// </summary>
        /// <param name="newState">The new state for this state machine</param>
        /// <exception cref="NullReferenceException"><paramref name="newState"/> is null</exception>
        public virtual void SetNextState(State newNextState)
        {
            newNextState.outer = this;
            newState = newNextState;
        }

        /// <summary>
        /// Sets the StateMachine's state to its mainStateType
        /// </summary>
        public virtual void SetNextStateToMain()
        {
            SetNextState(InitializeState((Type)mainStateType));
        }

        /// <summary>
        /// Runs <see cref="currentState"/>'s Update method
        /// </summary>
        protected virtual void Update()
        {
            currentState?.Update();
        }

        /// <summary>
        /// Runs <see cref="currentState"/>'s FixedUpdate method, if <see cref="newState"/> has a value, the newState is set.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            currentState?.FixedUpdate();

            if (newState != null)
                SetState(newState);
        }

        /// <summary>
        /// Runs <see cref="currentState"/>'s OnExit method.
        /// </summary>
        protected virtual void OnDestroy()
        {
            currentState?.OnExit();
        }

        public static TSM FindStateMachineByName<TSM>(GameObject obj, string name) where TSM : StateMachine
        {
            int hashCode = name.GetHashCode();
            return FindStateMachineByHashCode<TSM>(obj, hashCode);
        }
        public static TSM FindStateMachineByHashCode<TSM>(GameObject obj, int hashCode) where TSM : StateMachine
        {
            var stateMachines = obj.GetComponents<TSM>();
            foreach (var stateMachine in stateMachines)
            {
                if (stateMachine.stateMachineID == hashCode)
                    return stateMachine;
            }
            return null;
        }
    }
}