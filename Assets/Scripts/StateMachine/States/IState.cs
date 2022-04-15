namespace ViralVial
{
    public interface IState
    {
        public void Setup();
        public void UpdateState();
        public void Cleanup();

    }
}
