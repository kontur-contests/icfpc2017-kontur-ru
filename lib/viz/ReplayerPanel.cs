using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using lib.Scores.Simple;
using lib.viz.Detalization;

namespace lib.viz
{
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

        private bool showScore;

        public bool ShowScore
        {
            get { return showScore; }
            set
            {
                showScore = value;
                if (showScore) timer.Start();
                else timer.Stop();
            }
        }

        public void SetAugmentor(IPainterAugmentor augmentor)
        {
            mapPainter.PainterAugmentor = augmentor;
        }

        public void SetDataProvider(Map map, IReplayDataProvider newProvider)
        {
            provider = newProvider;
            mapPainter.Map = map;
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
            var scoreCalculator = new SimpleScoreCalculator();
            var scores = provider.PunterNames.Select(
                (name, i) => scoreCalculator.GetScore(i, gameState.CurrentMap, provider.GetPunterFutures(i)));
            scorePanel.SetScores(scores.ToArray());
        }
    }
}