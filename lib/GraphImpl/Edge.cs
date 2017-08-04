namespace lib.GraphImpl
{
    public class Edge
    {
        public readonly int To;
        public readonly int Owner;

        public Edge(int to, int owner)
        {
            To = to;
            Owner = owner;
        }
    }
}