namespace lib
{
    public interface ITransport
    {
        void Write(string data);
        string Read();
    }
}