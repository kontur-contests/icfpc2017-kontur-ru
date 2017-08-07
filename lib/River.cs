using System;
using Newtonsoft.Json;

namespace lib
{
    public class River : IEquatable<River>
    {
        [JsonProperty("source", Order = 1)] public readonly int Source;

        [JsonProperty("target", Order = 2)] public readonly int Target;

        [JsonProperty("owner", Order = 3)] public int Owner = -1;

        [JsonProperty("optionOwner", Order = 3)] public int OptionOwner = -1;

        public River()
        {
        }

        public River(int source, int target, int owner = -1, int optionOwner = -1)
        {
            Source = source;
            Target = target;
            Owner = owner;
            OptionOwner = optionOwner;
        }

        public bool Equals(River other)
        {
            return Source == other.Source && Target == other.Target 
                   || Source == other.Target && Target == other.Source;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(null, obj)) return false;
            if (Object.ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((River) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Source <= Target ? (Source * 397) ^ Target : (Target * 397) ^ Source;
            }
        }

        public static bool operator ==(River left, River right)
        {
            return Object.Equals(left, right);
        }

        public static bool operator !=(River left, River right)
        {
            return !Object.Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(Source)}: {Source}, {nameof(Target)}: {Target}, {nameof(Owner)}: {Owner}, {nameof(OptionOwner)}: {OptionOwner}";
        }

    }
}