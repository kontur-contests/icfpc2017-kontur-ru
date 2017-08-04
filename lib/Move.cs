using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lib
{
    public interface IMove
    {
    }

    public class AbstractMove : IMove
    {
        [JsonProperty("punter")]
        public int PunterId;
    }

    public class Pass : AbstractMove
    {
    }

    public class Move : AbstractMove, IEquatable<Move>
    {
        [JsonProperty("source")]
        public int Source;

        [JsonProperty("target")]
        public int Target;

        public Move()
        {
        }

        public Move(int punterId, int source, int target)
        {
            PunterId = punterId;
            Source = source;
            Target = target;
        }

        public bool Equals(Move other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Source == other.Source && Target == other.Target || Source == other.Target && Target == other.Source;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Move) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (Source < Target)
                    return (Source * 397) ^ Target;
                return (Target * 397) ^ Source;
            }
        }

        public static bool operator ==(Move left, Move right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(Source)}: {Source}, {nameof(Target)}: {Target}";
        }
    }
}