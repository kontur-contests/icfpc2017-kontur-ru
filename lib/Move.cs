using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib
{
    public enum MoveType
    {
        Claim,
        Pass
    }

    [TestFixture]
    public class Move_Should
    {
        [Test]
        public void BeClaim_WhenDeserializeFromClaim()
        {
            var json = "{\"claim\" : {\"punter\" : 123, \"source\" : 2, \"target\" : 1}}";

            JsonConvert.DeserializeObject<ClaimMove>(json);
        }

        [Test]
        public void BePass_WhenDeserializeFromPass()
        {
            var json = "{\"pass\" : {\"punter\" : 123}}";

            JsonConvert.DeserializeObject<PassMove>(json);
        }
    }

    public abstract class Move
    {
        public abstract Map Execute(Map map);
    }

    public class PassMove : Move
    {
        [JsonProperty("punter")] public int PunterId;

        public PassMove(int punterId)
        {
            PunterId = punterId;
        }

        public override Map Execute(Map map)
        {
            return map;
        }
    }


    public class ClaimMove : Move, IEquatable<ClaimMove>
    {
        [JsonProperty("punter")] public int PunterId;

        [JsonProperty("source")] public int Source;

        [JsonProperty("target")] public int Target;

        public ClaimMove(int punterId, int source, int target)
        {
            PunterId = punterId;
            Source = source;
            Target = target;
        }

        public override Map Execute(Map map)
        {
            var mapRivers = map.RiversList.Remove(new River(Source, Target)).Add(new River(Source, Target, PunterId));
            return new Map(map.Sites, mapRivers, map.Mines);
        }

        public bool Equals(ClaimMove other)
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
            return Equals((ClaimMove) obj);
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

        public static bool operator ==(ClaimMove left, ClaimMove right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClaimMove left, ClaimMove right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(Source)}: {Source}, {nameof(Target)}: {Target}";
        }
    }
}