namespace lib.StateImpl
{
    public interface IService
    {
        void Setup(State state, IServices services);
        void ApplyNextState(State state, IServices services);
    }
}