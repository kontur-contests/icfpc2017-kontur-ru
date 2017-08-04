using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.Scores.Simple
{
    public class SimpleScoreCalculator : IScoreCalculator
    {
        public long GetScore(int punter, Map map)
        {
            var distances = FloydAlgorithm.Run(map);

            var punterRivers = map.Rivers
                .Where(e => e.Owner == punter)
                .ToArray();

            var mapSize = map.Sites.Length;

            var disjointSetUnion = BuildDisjointSetUnion(mapSize, punterRivers);

            var allSites = GetAllSites(map, disjointSetUnion).ToArray();

            return allSites
                .Sum(e => distances[e.Item1][e.Item2] * distances[e.Item1][e.Item2]);
        }

        private IEnumerable<Tuple<int, int>> GetAllSites(Map map, DisjointSetUnion disjointSetUnion)
        {
            foreach (var mine in map.Mines)
            {
                foreach (var site in map.Sites)
                {
                    if (site.Id == mine)
                    {
                        continue;
                    }

                    if (disjointSetUnion.SameSet(site.Id, mine))
                    {
                        yield return Tuple.Create(site.Id, mine);
                    }
                }
            }
        }

        private static DisjointSetUnion BuildDisjointSetUnion(int mapSize, River[] punterRivers)
        {
            var disjointSetUnion = new DisjointSetUnion(mapSize);

            foreach (var river in punterRivers)
            {
                disjointSetUnion.Add(river.Target, river.Source);
            }

            return disjointSetUnion;
        }
    }
}