using System;
using System.Collections.Generic;

namespace Scripts.Common.Core.StateMachine
{
    public sealed class StateMachine<TStateId>
    {
        private readonly Dictionary<TStateId, IState<TStateId>> _states = new();

        public IState<TStateId> CurrentState { get; private set; }

        public event Action<TStateId, TStateId> OnStateChanged;

        public void Register(IState<TStateId> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            _states[state.StateId] = state;
        }

        public void Change(TStateId nextStateId)
        {
            if (!_states.TryGetValue(nextStateId, out var nextState))
            {
                throw new KeyNotFoundException($"State not registered: {nextStateId}");
            }

            if (CurrentState != null &&
                EqualityComparer<TStateId>.Default.Equals(CurrentState.StateId, nextStateId))
            {
                return;
            }

            var previousId = CurrentState != null ? CurrentState.StateId : default;
            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
            OnStateChanged?.Invoke(previousId, nextStateId);
        }

        public void Tick()
        {
            CurrentState?.Update();
        }
    }
}
