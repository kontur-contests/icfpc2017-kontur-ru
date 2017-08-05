namespace lib.GraphImpl
{
    public class Edge
    {
        public Edge(int from, int to, int owner)
            : this(new River(from, to, owner), EdgeDirection.Forward)
        {
        }

        private Edge(River river, EdgeDirection direction)
        {
            Direction = direction;
            River = river;
        }

        private EdgeDirection Direction { get; }
        public River River { get; }
        public int From => Direction == EdgeDirection.Forward ? River.Source : River.Target;
        public int To => Direction == EdgeDirection.Forward ? River.Target : River.Source;
        public int Owner => River.Owner;

        public static Edge Forward(River river) => new Edge(river, EdgeDirection.Forward);
        public static Edge Backward(River river) => new Edge(river, EdgeDirection.Backward);

        public Edge Reverse() => new Edge(
            River, Direction == EdgeDirection.Forward ? EdgeDirection.Backward : EdgeDirection.Forward);

        private enum EdgeDirection
        {
            Forward,
            Backward
        }
    }
}