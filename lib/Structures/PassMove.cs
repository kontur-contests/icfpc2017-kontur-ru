using System;

namespace lib.Structures
{
    public class PassMove : IEquatable<PassMove>
    {
        public int punter;

        public override string ToString()
        {
            return $"Pass id:{punter}";
        }

        public bool Equals(PassMove other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return punter == other.punter;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PassMove) obj);
        }

        public override int GetHashCode()
        {
            return punter;
        }

        public static bool operator ==(PassMove left, PassMove right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PassMove left, PassMove right)
        {
            return !Equals(left, right);
        }
    }
}