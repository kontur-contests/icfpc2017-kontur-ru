using System;
using System.Linq;

namespace lib.Score.Simple
{
    public static class FloydAlgorithm
    {
        public static int[][] Run(Map map)
        {
            var n = map.Sites.Length;
            var dist = Enumerable
                .Range(0, n)
                .Select(x => Enumerable.Repeat(int.MaxValue, n).ToArray())
                .ToArray();

            for (var i = 0; i < n; i++)
            {
                dist[i][i] = 0;
            }

            foreach (var river in map.Rivers)
            {
                dist[river.Source][river.Target] = 1;
                dist[river.Target][river.Source] = 1;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (var k = 0; k < n; k++)
                    {
                        var otherPath = dist[i][k] == int.MaxValue || dist[k][j] == int.MaxValue
                            ? int.MaxValue
                            : dist[i][k] + dist[k][j];

                        dist[i][j] = Math.Min(dist[i][j], otherPath);
                    }
                }
            }
            return dist;
        }
    }
}