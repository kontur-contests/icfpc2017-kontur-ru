namespace lib.StateImpl
{
    public interface IServices
    {
        T Get<T>() where T : IService;
    }
}