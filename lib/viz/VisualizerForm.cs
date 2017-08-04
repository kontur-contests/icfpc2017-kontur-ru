﻿using System;
using System.Linq;
using System.Windows.Forms;
using lib.Scores.Simple;
using lib.viz.Detalization;
using NUnit.Framework;

namespace lib.viz
{
    public class VisualizerForm : Form
    {
        private readonly Timer timer;
        private readonly MapPainter mapPainter;
        private readonly ScorePanel scorePanel;
        private readonly StartGameConfigPanel startGameConfigPanel;

        public VisualizerForm()
        {
            timer = new Timer();
            startGameConfigPanel = new StartGameConfigPanel
            {
                Dock = DockStyle.Left
            };
            startGameConfigPanel.SetMaps(MapLoader.LoadDefaultMaps().ToArray());
            startGameConfigPanel.SetAis(AiFactoryRegistry.Factories);

            mapPainter = new MapPainter
            {
                Map = startGameConfigPanel.SelectedMap.Map,
                PainterAugmentor = new DefaultPainterAugmentor()
            };

            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            var mapPanel = new ScaledViewPanel(mapPainter)
            {
                Dock = DockStyle.Fill
            };
            scorePanel = new ScorePanel { Dock = DockStyle.Top, Height = 40 };
            rightPanel.Controls.Add(scorePanel);
            rightPanel.Controls.Add(mapPanel);
            var simulator = new GameSimulator(startGameConfigPanel.SelectedMap.Map);

            startGameConfigPanel.MapChanged += map =>
            {
                simulator = new GameSimulator(startGameConfigPanel.SelectedMap.Map);
                mapPainter.Map = map.Map;
                mapPanel.Refresh();
            };

            var makeStepButton = new Button
            {
                Dock = DockStyle.Bottom,
                Text = "MAKE STEP"
            };

            startGameConfigPanel.AiSelected += factory =>
            {
                simulator.StartGame(startGameConfigPanel.SelectedAis);
                scorePanel.SetPlayers(startGameConfigPanel.SelectedAis);
            };
            makeStepButton.Click += (sender, args) =>
            {
                var gameState = simulator.NextMove();
                mapPainter.GameState = gameState;
                mapPanel.Refresh();
            };

            Controls.Add(rightPanel);
            Controls.Add(makeStepButton);
            Controls.Add(startGameConfigPanel);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            timer.Interval = 500;
            timer.Tick += OnTick;
            timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            timer.Stop();
            UpdateScores();
            timer.Start();
        }

        private void UpdateScores()
        {
            var gameState = mapPainter.GameState;
            if (gameState == null) return;
            var simpleScoreCalculator = new SimpleScoreCalculator();
            var scores = startGameConfigPanel.SelectedAis
                .Select(
                    (ai, i) => new GameSimulationResult(
                        ai, simpleScoreCalculator.GetScore(i, gameState.CurrentMap)));
            scorePanel.SetScores(scores.ToArray());
        }
    }

    public class AiFactoryRegistry
    {
        public static readonly AiFactory[] Factories;

        static AiFactoryRegistry()
        {
            var types = typeof(AiFactoryRegistry).Assembly.GetTypes()
                .Where(x => typeof(IAi).IsAssignableFrom(x) && x.GetConstructor(Type.EmptyTypes) != null);
            Factories = types.Select(type => new AiFactory(type.Name, () => (IAi) Activator.CreateInstance(type)))
                .ToArray();
        }
    }

    [TestFixture]
    public class VisualizerForm_Test
    {
        [Explicit]
        [Test]
        [STAThread]
        public void Test()
        {
            var form = new VisualizerForm();
            form.ShowDialog();
        }
    }
}