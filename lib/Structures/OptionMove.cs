using System;

namespace lib.Structures
{
    public class OptionMove : IEquatable<OptionMove>
    {
        public int punter;
        public int source;
        public int target;

        public bool Equals(OptionMove other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return punter == other.punter && (source == other.source && target == other.target || source == other.target && target == other.source);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((OptionMove)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var s = source;
                var t = target;
                if (s < t)
                {
                    s = target;
                    t = source;
                }
                var hashCode = punter;
                hashCode = (hashCode * 397) ^ s;
                hashCode = (hashCode * 397) ^ t;
                return hashCode;
            }
        }

        public static bool operator ==(OptionMove left, OptionMove right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OptionMove left, OptionMove right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"Option id:{punter} {source}-{target}";
        }
    }

}