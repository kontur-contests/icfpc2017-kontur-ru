using System;
using System.Collections.Immutable;
using System.Linq;
using lib.StateImpl;
using lib.Structures;
using Newtonsoft.Json;

namespace lib
{
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

        public Map ApplyMove(AiInfoMoveDecision decision)
        {
            return ApplyMove(decision.move);
        }

        public Map ApplyMove(Move move)
        {
            if (move.claim != null)
                return ApplyClaim(move.claim.punter, move.claim.source, move.claim.target, move);

            var current = this;
            if (move.splurge != null && move.splurge.SplurgeLength() > 0)
            {
                var old = move.splurge.route[0];
                foreach (var step in move.splurge.route.Skip(1))
                {
                    current = current.ApplyClaim(move.splurge.punter, old, step, move);
                    old = step;
                }
                return current;
            }
            return current;
        }

        private Map ApplyClaim(int punterId, int source, int target, Move move)
        {
            var oldRiver = new River(source, target);
            var actualRiver = RiversList.FirstOrDefault(r => r.Equals(oldRiver))
                              ?? throw new InvalidOperationException($"Try to claim unexistent river {source}--{target}. Move: {move}");
            if (actualRiver.Owner != -1)
                throw new InvalidOperationException($"Try to claim river {actualRiver}. Move: {move}");
            var mapRivers = RiversList.Remove(oldRiver).Add(new River(source, target, punterId));
            return new Map(Sites, mapRivers, Mines);
        }

        public string Md5Hash()
        {
            return JsonConvert.SerializeObject(this, Formatting.None).CalculateMd5();
        }

        public Map(Site[] sites, River[] rivers, int[] mines)
            : this(sites, rivers.ToImmutableHashSet(), mines)
        {
        }
        public Map(Site[] sites, ImmutableHashSet<River> rivers, int[] mines)
        {
            Sites = sites;
            RiversList = rivers;
            Mines = mines;
        }
    }
}