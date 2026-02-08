using System;
using System.Collections.Generic;
using Services.GameStates.StateMachine.Interfaces;
using UnityEditor;
using Zenject;

namespace Services.GameStates.StateMachine
{
    public class GameStateMachine
    {
        public IState ActiveState { get; private set; }

        private readonly Dictionary<Type, IState> _states = new();

        private IInstantiator _instantiator;

        public GameStateMachine(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }
        
        public void Enter<TState>() where TState : class, IState
        {
            TState state = ChangeState<TState>(new object[]{});
            state.Enter();
        }
        
        public void Enter<TState>(IEnumerable<object> extraArgs) where TState : class, IState
        {
            TState state = ChangeState<TState>(extraArgs);
            state.Enter();
        }
        
        private TState ChangeState<TState>(IEnumerable<object> extraArgs) where TState : class, IState
        {
            ActiveState?.Exit();
            TState state = GetState<TState>(extraArgs);
            ActiveState = state;
            return state;
        }
        
        private TState GetState<TState>(IEnumerable<object> extraArgs) where TState : class, IState
        {
            Type key = typeof(TState);
            
            IList<object> argsList = extraArgs as IList<object> ?? new List<object>(extraArgs);
            if (argsList.Count > 0)
            {
                return _instantiator.Instantiate<TState>(argsList);
            }

            if (_states.TryGetValue(key, out IState cached))
                return (TState)cached;

            TState created = _instantiator.Instantiate<TState>(extraArgs);
            _states[key] = created;
            return created;
        }
    }
}