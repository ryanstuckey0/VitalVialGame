namespace ViralVial
{
    public class BaseState : IState
    {
        public StateMachine StateMachine;

        public virtual void Cleanup() { }
        public virtual void Setup() { }
        public virtual void UpdateState() { }
    }
}
