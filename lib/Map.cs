using System.Collections.Immutable;
using System.Linq;
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

        public Map ApplyMove(Move move)
        {
            if (move.claim == null)
                return this;
            var mapRivers = RiversList.Remove(new River(move.claim.source, move.claim.target)).Add(new River(move.claim.source, move.claim.target, move.claim.punter));
            return new Map(Sites, mapRivers, Mines);
        }

        public string Md5Hash()
        {
            return JsonConvert.SerializeObject(this, Formatting.None).CalculateMd5();
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
}