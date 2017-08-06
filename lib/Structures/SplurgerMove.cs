using System;
using System.Linq;

namespace lib.Structures
{
    public class SplurgerMove : IEquatable<SplurgerMove>
    {
        public int punter;
        public int[] route;

        public int SplurgeLength() => route?.Length - 1 ?? 0;

        public bool Equals(SplurgerMove other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return punter == other.punter && route.SequenceEqual(other.route);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SplurgerMove)obj);
        }

        public override int GetHashCode()
        {
            return route.Aggregate(0, (o, n) => o ^ n);
        }

        public static bool operator ==(SplurgerMove left, SplurgerMove right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SplurgerMove left, SplurgerMove right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"Splurge id:{punter} {string.Join("--", route)}";
        }
    }
}