namespace lib.Interaction
{
    public interface ITransport
    {
        void Write(string data);
        string Read(int? timeout = null);
    }
}