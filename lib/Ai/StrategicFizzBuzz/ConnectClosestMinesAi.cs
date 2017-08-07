using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using lib.StateImpl;
using lib.Strategies;
using lib.Structures;
using lib.viz;
using NUnit.Framework;

namespace lib.Ai.StrategicFizzBuzz
{
    public class ConnectClosestMinesAi : CompositeStrategicAi
    {
        public ConnectClosestMinesAi()
            : base(
                (state, services) => new ExtendComponentStrategy(true, state, services),
                (state, services) => new BuildNewComponentStrategy(true, state, services),
                (state, services) => new GreedyStrategy(true, state, services, Math.Max))
        {
        }

        public override string Version => "1.0";
    }

    [TestFixture]
    public class ConnectClosestMinesAi_Should
    {
        [Test]
        public void Test1()
        {
            var ai = new ConnectClosestMinesAi();
            var state = new State{punter = 0, punters = 1, map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\sample.json")).Map };
            ai.Setup(state, new Services(state));
            var moveDecision = ai.GetNextMove(state, new Services(state));
            Assert.That(moveDecision.move, Is.EqualTo(Move.Claim(0, 5, 3)));
            state.map = state.map.ApplyMove(moveDecision.move);
            state.turns.Add(new TurnState());
            moveDecision = ai.GetNextMove(state, new Services(state));
            Assert.That(moveDecision.move, Is.EqualTo(Move.Claim(0, 1, 3)));
            state.map = state.map.ApplyMove(moveDecision.move);
            state.turns.Add(new TurnState());
            moveDecision = ai.GetNextMove(state, new Services(state));
            Assert.That(moveDecision.move, Is.EqualTo(Move.Claim(0, 0, 1)));
        }
        
        [Test]
        [STAThread]
        [Explicit]
        public void Show()
        {
            var form = new Form();
            var painter = new MapPainter();
            var map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\sample.json"));

            var ai = new ConnectClosestMinesAi();
            var simulator = new GameSimulator(map.Map, new Settings());
            simulator.StartGame(new List<IAi> { ai });
            var gameState = simulator.NextMove();
            painter.Map = gameState.CurrentMap;

            var panel = new ScaledViewPanel(painter)
            {
                Dock = DockStyle.Fill
            };
            form.Controls.Add(panel);
            form.ShowDialog();
        }
    }
}