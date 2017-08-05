namespace lib.Structures
{
    public class HandshakeIn : InBase
    {
        public string you;

        public override string ToString()
        {
            return $"{nameof(you)}: {you}";
        }
    }
}