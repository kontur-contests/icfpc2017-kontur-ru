namespace lib.GraphImpl
{
    public class Edge
    {
        public readonly int From;
        public readonly int To;
        public readonly int Owner;

        public Edge(int from, int to, int owner)
        {
            From = from;
            To = to;
            Owner = owner;
        }
    }
}