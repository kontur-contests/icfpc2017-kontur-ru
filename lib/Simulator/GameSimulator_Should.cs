using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using lib.Ai;
using lib.Structures;
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
            map = MapLoader.LoadMapByName("sample.json");
            simulator = new GameSimulator(map.Map, new Settings());
        }

        [Test]
        public void OwnRiverTest()
        {
            var gamer = new Ai();
            simulator.StartGame(new List<IAi> {gamer});

            var state = simulator.NextMove();

            Assert.AreEqual(0, state.CurrentMap.Rivers.First(x => x.Source == 0 && x.Target == 1).Owner);
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

            public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
            {
                return new Future[0];
            }

            public Move GetNextMove(Move[] prevMoves, Map map)
            {
                return Move.Claim(0, 0, 1);
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