using System;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;

namespace lib
{
    public class Site
    {
        [JsonProperty("id")] public int Id;

        [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)] public float X;

        [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)] public float Y;

        public Site()
        {
        }

        public Site(int id, float x, float y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }
    }

    public class Map
    {
        [JsonProperty("mines", Order = 3)] public int[] Mines = new int[0];

        [JsonIgnore] public ImmutableHashSet<River> RiversList = ImmutableHashSet<River>.Empty;

        [JsonProperty("rivers", Order = 2)]
        public River[] Rivers
        {
            get => RiversList.ToArray();
            set => RiversList = value.ToImmutableHashSet();
        }

        [JsonProperty("sites", Order = 1)] public Site[] Sites = new Site[0];

        public Map()
        {
        }

        public Map(Site[] sites, River[] rivers, int[] mines)
            :this(sites, rivers.ToImmutableHashSet(), mines)
        {
        }
        public Map(Site[] sites, ImmutableHashSet<River> rivers, int[] mines)
        {
            Sites = sites;
            RiversList = rivers;
            Mines = mines;
        }
    }

    public class River : IEquatable<River>
    {
        [JsonProperty("source", Order = 1)] public readonly int Source;

        [JsonProperty("target", Order = 2)] public readonly int Target;

        [JsonProperty("owner", Order = 3)] public int Owner = -1;

        public River()
        {
        }

        public River(int source, int target, int owner = -1)
        {
            Source = source;
            Target = target;
            Owner = owner;
        }

        public bool Equals(River other)
        {
            return Source == other.Source && Target == other.Target 
                || Source == other.Target && Target == other.Source;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
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
            return Equals(left, right);
        }

        public static bool operator !=(River left, River right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(Source)}: {Source}, {nameof(Target)}: {Target}, {nameof(Owner)}: {Owner}";
        }

    }
}