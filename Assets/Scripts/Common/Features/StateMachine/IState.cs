namespace Scripts.Common.Features.StateMachine
{
    public interface IState<TStateId>
    {
        TStateId StateId { get; }
        void Enter();
        void Update();
        void Exit();
    }
}
