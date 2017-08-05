using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using lib.GraphImpl;
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
        private FuturesPanel futuresPanel;

        public ReplayerPanel()
        {
            mapPainter = new MapPainter
            {
                PainterAugmentor = new DefaultPainterAugmentor {ShowFutures = true}
            };
            var mapPanel = new ScaledViewPanel(mapPainter)
            {
                Dock = DockStyle.Fill
            };
            scorePanel = new ScorePanel {Dock = DockStyle.Top, Height = 40};

            var navigationPanel = new TableLayoutPanel {Dock = DockStyle.Bottom};

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

            navigationPanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.2f});
            navigationPanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.6f});
            navigationPanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.2f});
            navigationPanel.Controls.Add(revertStepButton, 0, 0);
            navigationPanel.Controls.Add(gameProgress, 1, 0);
            navigationPanel.Controls.Add(makeStepButton, 2, 0);

            makeStepButton.Click += (_, args) =>
            {
                if (provider == null) return;
                if (current + 1 >= states.Count)
                    if (!TryGenerateNextState())
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

            futuresPanel = new FuturesPanel();
            futuresPanel.FuturesVisibleChanged += v => mapPainter.PainterAugmentor.ShowFutures = v;
            futuresPanel.FuturesVisibleChanged += _ => Refresh();

            var middlePanel = new TableLayoutPanel {Dock = DockStyle.Fill};
            middlePanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.8f});
            middlePanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.2f});
            middlePanel.Controls.Add(mapPanel, 0, 0);
            middlePanel.Controls.Add(futuresPanel, 1, 0);
            Controls.Add(middlePanel);

            Controls.Add(navigationPanel);
            timer = new Timer {Interval = 500};
            timer.Tick += OnTick;
            timer.Start();
        }

        private bool liveScoreUpdate;
        private int current;
        private readonly TrackBar gameProgress;

        public bool LiveScoreUpdate
        {
            get => liveScoreUpdate;
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
            provider = newProvider;

            current = -1;
            states.Clear();

            var playersCount = newProvider.PunterNames.Length;
            mapPainter.Futures = Enumerable.Range(0, playersCount).ToDictionary(i => i, newProvider.GetPunterFutures);
            futuresPanel.SetFutures(mapPainter.Futures, new Graph(map));

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