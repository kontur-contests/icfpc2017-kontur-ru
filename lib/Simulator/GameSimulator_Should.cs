using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace lib
{
    [TestFixture]
    public class GameSimulator_Should
    {
        private NamedMap map;
        private GameSimulator simulator;


        [SetUp]
        public void SetUp()
        {
            map = MapLoader.LoadMap(Path.Combine(MapLoader.DefaultPath, "sample.json"));
            simulator = new GameSimulator(map.Map);
        }

        [Test]
        public void OwnRiverTest()
        {
            var gamer = new Ai();
            simulator.StartGame(new List<IAi> {gamer});

            simulator.NextMove();

            Assert.AreEqual(0, map.Map.Rivers.First(x => x.Source == 0 && x.Target == 1).Owner);
        }

        [Test]
        public void MakeTwoMoves()
        {
            var gamer1 = new Ai();
            simulator.StartGame(new List<IAi> {gamer1});

            Action a = () => simulator.NextMove();

            a();
            a.ShouldNotThrow();
        }

        private class Ai : IAi
        {
            public string Name { get; }

            public void StartRound(int punterId, int puntersCount, Map map)
            {
            }

            public IMove GetNextMove(IMove[] prevMoves, Map map)
            {
                return new Move(0, 0, 1);
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
    }
}