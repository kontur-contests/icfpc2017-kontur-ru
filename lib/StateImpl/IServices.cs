namespace lib.StateImpl
{
    public interface IServices
    {
        T Get<T>(State state) where T : IService, new();
        void ApplyNextState(State state);
    }
}