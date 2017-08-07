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

        public ImmutableDictionary<int, int> OptionsUsed = ImmutableDictionary<int, int>.Empty;

        public int OptionsLeft(int punter) => Mines.Length - OptionsUsed.GetOrDefaultNoSideEffects(punter, 0);

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
            if (move.option != null)
                return ApplyOption(move.option.punter, move.option.source, move.option.target, move);
            var current = this;
            if (move.splurge != null && move.splurge.SplurgeLength() > 0)
            {
                var old = move.splurge.route[0];
                foreach (var step in move.splurge.route.Skip(1))
                {
                    current = current.ApplySplurgeItem(move.splurge.punter, old, step, move);
                    old = step;
                }
                return current;
            }
            return current;
        }

        private Map ApplyOption(int punterId, int source, int target, Move move)
        {
            var oldRiver = new River(source, target);
            var actualRiver = RiversList.FirstOrDefault(r => r.Equals(oldRiver))
                              ?? throw new InvalidOperationException($"Try to claim unexistent river {source}--{target}. Move: {move}");
            if (actualRiver.Owner == -1)
                throw new InvalidOperationException($"Try to buy option of river without owner {actualRiver}. Move: {move}");
            if (actualRiver.OptionOwner != -1)
                throw new InvalidOperationException($"Try to buy option of river with option owner {actualRiver}. Move: {move}");
            if (actualRiver.Owner == punterId)
                throw new InvalidOperationException($"Try to buy option for self-owned rived. Move: {move}");
            int punterOptionsUsed = OptionsUsed.GetOrDefaultNoSideEffects(punterId, 0);
            if (punterOptionsUsed >= Mines.Length)
                throw new InvalidOperationException($"Try to buy option while there are no options left. Move: {move}");
            var mapRivers = RiversList.Remove(oldRiver).Add(new River(source, target, actualRiver.Owner, punterId));
            return new Map(
                Sites, mapRivers, Mines, 
                OptionsUsed.SetItem(punterId, punterOptionsUsed + 1));
        }

        private Map ApplyClaim(int punterId, int source, int target, Move move)
        {
            var oldRiver = new River(source, target);
            var actualRiver = RiversList.FirstOrDefault(r => r.Equals(oldRiver))
                              ?? throw new InvalidOperationException($"Try to claim unexistent river {source}--{target}. Move: {move}");
            if (actualRiver.Owner != -1)
                throw new InvalidOperationException($"Try to claim river {actualRiver}. Move: {move}");
            var mapRivers = RiversList.Remove(oldRiver).Add(new River(source, target, punterId));
            return new Map(Sites, mapRivers, Mines, OptionsUsed);
        }

        private Map ApplySplurgeItem(int punterId, int source, int target, Move move)
        {
            var oldRiver = new River(source, target);
            var actualRiver = RiversList.FirstOrDefault(r => r.Equals(oldRiver))
                              ?? throw new InvalidOperationException($"Try to claim unexistent river {source}--{target}. Move: {move}");
            if (actualRiver.Owner == -1)
            {
                var mapRivers = RiversList.Remove(oldRiver).Add(new River(source, target, punterId));
                return new Map(Sites, mapRivers, Mines, OptionsUsed);
            }
            if (actualRiver.OptionOwner == -1)
            {
                if (actualRiver.Owner == punterId)
                    throw new InvalidOperationException($"Try to buy option for self-owned rived. Move: {move}");
                int punterOptionsUsed = OptionsUsed.GetOrDefaultNoSideEffects(punterId, 0);
                if (punterOptionsUsed >= Mines.Length)
                    throw new InvalidOperationException($"Try to buy option while there are no options left. Move: {move}");
                var mapRivers = RiversList.Remove(oldRiver).Add(new River(source, target, actualRiver.Owner, punterId));
                return new Map(
                    Sites, mapRivers, Mines,
                    OptionsUsed.SetItem(punterId, punterOptionsUsed + 1));
            }
            throw new InvalidOperationException($"Try to splurge river with option owner {actualRiver}. Move: {move}");
        }

        public string Md5Hash()
        {
            return JsonConvert.SerializeObject(this, Formatting.None).CalculateMd5();
        }

        public Map(Site[] sites, River[] rivers, int[] mines)
            : this(sites, rivers.ToImmutableHashSet(), mines, ImmutableDictionary<int, int>.Empty)
        {
        }

        public Map(Site[] sites, ImmutableHashSet<River> rivers, int[] mines, ImmutableDictionary<int, int> optionsUsed)
        {
            Sites = sites;
            RiversList = rivers;
            Mines = mines;
            OptionsUsed = optionsUsed;
        }
    }
}