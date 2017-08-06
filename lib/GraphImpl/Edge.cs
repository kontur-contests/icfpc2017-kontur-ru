using System;

namespace lib.GraphImpl
{
    public class Edge : IEquatable<Edge>
    {
        public Edge(int from, int to, int owner)
            : this(new River(from, to, owner), from, to)
        {
        }

        private Edge(River river, int from, int to)
        {
            River = river;
            From = from;
            To = to;
        }

        public River River { get; }
        public int From { get; }
        public int To { get; }
        public int Owner => River.Owner;

        public static Edge Forward(River river) => new Edge(river, river.Source, river.Target);
        public static Edge Backward(River river) => new Edge(river, river.Target, river.Source);

        public Edge Reverse() => new Edge(River, To, From);

        public override string ToString()
        {
            return $"{nameof(From)}: {From}, {nameof(To)}: {To}, {nameof(Owner)}: {Owner}";
        }

        #region Equality members

        public bool Equals(Edge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return From == other.From && To == other.To;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Edge) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (From * 397) ^ To;
            }
        }

        public static bool operator ==(Edge left, Edge right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}