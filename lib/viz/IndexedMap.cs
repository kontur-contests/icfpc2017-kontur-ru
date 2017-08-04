using System.Collections.Generic;
using System.Linq;
using lib;

namespace CinemaLib
{
    public class IndexedMap
    {
        public IndexedMap(Map map)
        {
            Map = map;
            SiteById = map.Sites.ToDictionary(x => x.Id);
            MineIds = new HashSet<int>(map.Mines);
        }

        public Dictionary<int, Site> SiteById { get; }
        public HashSet<int> MineIds { get; }
        public Map Map { get; }

        public Site[] Sites => Map.Sites;
        public River[] Rivers => Map.Rivers;
    }
}