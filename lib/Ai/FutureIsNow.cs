using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.Structures;
using lib.viz;
using lib.viz.Detalization;
using MoreLinq;
using NUnit.Framework;

namespace lib.Ai
{
    public class FutureIsNow : IAi
    {
        public string Name => "Futurer";
        public string Version => "0";
        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            throw new System.NotImplementedException(); 
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            throw new System.NotImplementedException();
        }

        public string SerializeGameState()
        {
            throw new System.NotImplementedException();
        }

        public void DeserializeGameState(string gameState)
        {
            throw new System.NotImplementedException();
        }
    }

    [TestFixture]
    public class FutureIsNow_Should
    {
        [Test]
        [Explicit]
        public void SelectMines()
        {
            var map = MapLoader.LoadMapByNameInTests("tube.json").Map;
            var desiredLen = 5;
            var path = new PathSelector(map).SelectPath(desiredLen);
            //Assert.AreEqual(desiredLen, path.Length);
            map.ShowWithPath(path);

        }
    }

    public class PathSelector
    {
        private readonly Map map;
        private Graph graph;
        private MineDistCalculator minDists;

        public PathSelector(Map map)
        {
            this.map = map;
            graph = new Graph(map);
            minDists = new MineDistCalculator(graph);
        }

        public int[] SelectPath(int length)
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
            return closestTriple.p12.Concat(closestTriple.p23.Skip(1)).Take(length+1).ToArray();
        }
    }
}