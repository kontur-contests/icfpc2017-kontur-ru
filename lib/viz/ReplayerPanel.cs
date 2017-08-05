using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
        private readonly List<GameState> states = new List<GameState>();

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

            var buttonPanel = new TableLayoutPanel {Dock = DockStyle.Bottom};

            var revertStepButton = new Button
            {
                Dock = DockStyle.Fill,
                Text = "REVERT STEP"
            };
            gameProgress = new TrackBar
            {
                Dock = DockStyle.Fill,
                Maximum = 0,
                Minimum = 0
            };
            var makeStepButton = new Button
            {
                Dock = DockStyle.Fill,
                Text = "MAKE STEP"
            };

            buttonPanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.2f});
            buttonPanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.6f});
            buttonPanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.2f});
            buttonPanel.Controls.Add(revertStepButton, 0, 0);
            buttonPanel.Controls.Add(gameProgress, 1, 0);
            buttonPanel.Controls.Add(makeStepButton, 2, 0);

            makeStepButton.Click += (_, args) =>
            {
                if (provider == null) return;
                if (current + 1 >= states.Count)
                    if(!TryGenerateNextState())
                        return;
                current++;
                mapPainter.GameState = states[current];
                mapPanel.Refresh();
                gameProgress.Value = current;
            };
            gameProgress.ValueChanged += (_, args) =>
            {
                current = gameProgress.Value;
                if (current < 0 || current >= states.Count)
                    return;
                mapPainter.GameState = states[current];
                mapPanel.Refresh();
            };
            revertStepButton.Click += (_, args) =>
            {
                if (current <= 0)
                    return;
                current--;
                mapPainter.GameState = states[current];
                mapPanel.Refresh();
                gameProgress.Value = current;
            };
            Controls.Add(scorePanel);
            Controls.Add(mapPanel);
            Controls.Add(buttonPanel);
            timer = new Timer { Interval = 500 };
            timer.Tick += OnTick;
            timer.Start();
        }

        private bool liveScoreUpdate;
        private int current;
        private readonly TrackBar gameProgress;

        public bool LiveScoreUpdate
        {
            get { return liveScoreUpdate; }
            set
            {
                liveScoreUpdate = value;
                if (liveScoreUpdate) timer.Start();
                else timer.Stop();
            }
        }

        public void SetAugmentor(IPainterAugmentor augmentor)
        {
            mapPainter.PainterAugmentor = augmentor;
        }

        public void SetDataProvider(Map map, IReplayDataProvider newProvider)
        {
            current = -1;
            states.Clear();
            provider = newProvider;
            mapPainter.Map = map;
            mapPainter.GameState = null;
            scorePanel.SetPlayers(newProvider.PunterNames);
            Refresh();
            if (newProvider is LogReplayDataProvider)
            {
                while (TryGenerateNextState())
                { }
                current = 0;
            }
        }

        private bool TryGenerateNextState()
        {
            if (states.Count != 0 && states.Last().IsGameOver)
                return false;
            states.Add(provider.NextMove());
            gameProgress.Maximum = states.Count - 1;
            return true;
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