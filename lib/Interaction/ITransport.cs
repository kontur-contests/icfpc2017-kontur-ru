using lib.Structures;

namespace lib.Interaction
{
    public interface ITransport
    {
        void Write<T>(T data);
        T Read<T>(int timeout = 15000) where T : InBase;
    }
}