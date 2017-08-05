using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using lib.Scores.Simple;
using lib.viz.Detalization;

namespace lib.viz
{
    public interface IReplayDataProvider
    {
        string[] PunterNames { get; }
        GameState NextMove();
    }

    public class SimulatorReplayDataProvider : IReplayDataProvider
    {
        private readonly List<IAi> ais;
        private readonly GameSimulator simulator;

        public SimulatorReplayDataProvider(List<IAi> ais, Map map)
        {
            this.ais = ais;
            simulator = new GameSimulator(map);
            simulator.StartGame(ais);
        }

        public string[] PunterNames => ais.Select(ai => ai.Name).ToArray();
        public GameState NextMove()
        {
            return simulator.NextMove();
        }
    }

    public class ReplayerPanel : Panel
    {
        private readonly Timer timer;
        private readonly MapPainter mapPainter;
        private readonly ScorePanel scorePanel;
        private IReplayDataProvider provider;

        public ReplayerPanel()
        {
            mapPainter = new MapPainter
            {
                // Map = startGameConfigPanel.SelectedMap.Map,
                PainterAugmentor = new DefaultPainterAugmentor()
            };
            var mapPanel = new ScaledViewPanel(mapPainter)
            {
                Dock = DockStyle.Fill
            };
            scorePanel = new ScorePanel { Dock = DockStyle.Top, Height = 40 };
            var makeStepButton = new Button
            {
                Dock = DockStyle.Bottom,
                Text = "MAKE STEP"
            };

            makeStepButton.Click += (sender, args) =>
            {
                if (provider == null) return;
                var gameState = provider.NextMove();
                mapPainter.GameState = gameState;
                mapPanel.Refresh();
            };
            Controls.Add(scorePanel);
            Controls.Add(mapPanel);
            Controls.Add(makeStepButton);
            timer = new Timer { Interval = 500 };
            timer.Tick += OnTick;
            timer.Start();
        }

        public void SetDataProvider(Map map, IReplayDataProvider newProvider)
        {
            provider = newProvider;
            mapPainter.Map = map.Clone();
            mapPainter.GameState = null;
            scorePanel.SetPlayers(newProvider.PunterNames);
            Refresh();
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
            var scores = provider.PunterNames.Select(
                (name, i) => simpleScoreCalculator.GetScore(i, gameState.CurrentMap));
            scorePanel.SetScores(scores.ToArray());
        }
    }
}