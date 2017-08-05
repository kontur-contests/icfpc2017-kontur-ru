using System.Collections.Generic;
using FluentAssertions;
using System.Linq;
using lib.GraphImpl;
using MoreLinq;
using NUnit.Framework;

namespace lib.Ai
{
    public class PathSelector
    {
        private readonly Map map;
        private Graph graph;
        private MineDistCalculator minDists;

        public PathSelector(Map map, MineDistCalculator minDists)
        {
            this.map = map;
            graph = new Graph(map);
            this.minDists = minDists;
        }

        public List<int> SelectPath(int length)
        {
            if (map.Mines.Length <= 1) return new List<int>();
            if (map.Mines.Length == 2)
            {
                var allPairs =
                    from m1 in map.Mines
                    from m2 in map.Mines
                    where m1 != m2
                    let p12 = minDists.GetPath(m1, m2).Enumerate().Reverse().ToList()
                    select new { m1, m2, p12 };
                var closestTuple = allPairs.MinBy(t => t.p12.Count);
                return closestTuple.p12.Take(length + 1).ToList();
            }
            else
            {
                var allPairs =
                    from m1 in map.Mines
                    from m2 in map.Mines
                    from m3 in map.Mines
                    where m1 != m2 && m2 != m3 && m1 != m3
                    let p12 = minDists.GetPath(m1, m2).Enumerate().Reverse().ToList()
                    let p23 = minDists.GetPath(m2, m3).Enumerate().Reverse().ToList()
                    select new { m1, m2, m3, p12, p23 };
                var closestTriple = allPairs.MinBy(t => t.p12.Count + t.p23.Count);
                return closestTriple.p12.Concat(closestTriple.p23.Skip<int>(1)).Take(length + 1)
                    .ToList();
            }
        }
    }

    [TestFixture]
    public class PathSelector_Should
    {
        [Test]
        [Explicit]
        public void SelectMines()
        {
            var map = MapLoader.LoadMapByNameInTests("tube.json").Map;
            var desiredLen = 15;
            var minDists = new MineDistCalculator(new Graph(map));
            var path = new PathSelector(map, minDists).SelectPath(desiredLen);
            var positioner = new FuturesPositioner(map, path, minDists);
            var futures = positioner.GetFutures();
            map.ShowWithPath(path, futures);
        }

        [Test]
        public void SelectMinesOnTube()
        {
            var map = MapLoader.LoadMapByNameInTests("tube.json").Map;
            var desiredLen = 12;
            var path = new PathSelector(map, new MineDistCalculator(new Graph(map))).SelectPath(desiredLen);
            Assert.AreEqual(desiredLen + 1, path.Count);
            path.Should().Contain(new[] { 285, 260, 148, 64 });
        }
    }
}