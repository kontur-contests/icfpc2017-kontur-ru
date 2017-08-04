using System;
using System.Linq;
using NUnit.Framework;

namespace lib
{
    public class ScoreCalculator
    {
        public int GetScore(int punter, Map map)
        {
            var distances = GetAllDistances(map);

            var punterRivers = map.Rivers
                .Where(e => e.Owner == punter)
                .ToArray();

            var disjointSetUnion = new DisjointSetUnion(map.Sites.Length);

            foreach (var river in punterRivers)
            {
                disjointSetUnion.Add(river.Target, river.Source);
            }

            var sum = 0;

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
                        sum += distances[site.Id][mine] * distances[site.Id][mine];
                    }
                }
            }

            return sum;
        }

        private int[][] GetAllDistances(Map map)
        {
            var n = map.Sites.Length;
            var dist = Enumerable
                .Range(0, n)
                .Select(x => Enumerable.Repeat(int.MaxValue, n).ToArray())
                .ToArray();

            foreach (var river in map.Rivers)
            {
                dist[river.Source][river.Target] = 1;
                dist[river.Target][river.Source] = 1;
            }
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (var k = 0; k < n; k++)
                        dist[i][j] = Math.Min(dist[i][j], dist[i][k] + dist[k][j]);
            return dist;
        }

        public class DisjointSetUnion
        {
            public int[] root;
            public int[] treeSize;

            public DisjointSetUnion(int size)
            {
                root = Enumerable.Repeat(-1, size).ToArray();
                treeSize = Enumerable.Repeat(1, size).ToArray();
            }

            public bool SameSet(int x, int y)
            {
                var xRoot = Root(x);
                var yRoot = Root(y);
                return xRoot == yRoot;
            }

            public bool Add(int x, int y)
            {
                var xRoot = Root(x);
                var yRoot = Root(y);
                if (xRoot == yRoot)
                    return false;
                if (treeSize[xRoot] < treeSize[yRoot])
                {
                    var t = yRoot;
                    yRoot = xRoot;
                    xRoot = t;
                }
                treeSize[xRoot] += treeSize[yRoot];
                root[y] = x;
                return true;
            }

            private int Root(int x)
            {
                return root[x] < 0 ? x : (root[x] = Root(root[x]));
            }
        }
    }
}