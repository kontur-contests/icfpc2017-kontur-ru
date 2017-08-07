using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using JetBrains.Annotations;
using lib.Scores.Simple;
using lib.Strategies;
using lib.Structures;
using lib.viz;
using NUnit.Framework;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    [ShouldNotRunOnline]
    public class CrazyAi : CompositeStrategicAi
    {
        public CrazyAi()
            : base(
                (state, services) => new CrazyStrategy(state, services),
                (state, services) => new GreedyStrategy(true, state, services, Math.Max))
        {
        }

        public override string Version => "0.1";
    }

    [TestFixture]
    public class CrazyAi_Run
    {
        [Test]
        [STAThread]
        [Explicit]
        public void Show()
        {
            var form = new Form();
            var painter = new MapPainter();
            var map = MapLoader.LoadMap(
                Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\sample.json"));

            var ai = new GreedyAi();
            var simulator = new GameSimulator(map.Map, new Settings());
            simulator.StartGame(new List<IAi> {ai});

            while (true)
            {
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

        [Test]
        public void Test1()
        {
            var gamers = new List<IAi> {new CrazyAi(), new ConnectClosestMinesAi()};
            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());

            var results = gameSimulator.SimulateGame(
                gamers, MapLoader.LoadMapByName("edge.json").Map, new Settings());

            foreach (var gameSimulationResult in results)
                Console.Out.WriteLine(
                    "gameSimulationResult = {0}:{1}", gameSimulationResult.Gamer.Name, gameSimulationResult.Score);
        }
    }
}