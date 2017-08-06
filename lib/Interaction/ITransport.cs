using lib.Structures;

namespace lib.Interaction
{
    public interface ITransport
    {
        void Write<T>(T data);
        T Read<T>(int? timeout = null) where T : InBase;
    }
}