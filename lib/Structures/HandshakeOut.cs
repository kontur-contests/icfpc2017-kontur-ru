namespace lib.Structures
{
    public class HandshakeOut
    {
        public string me;

        public override string ToString()
        {
            return $"{nameof(me)}: {me}";
        }
    }
}