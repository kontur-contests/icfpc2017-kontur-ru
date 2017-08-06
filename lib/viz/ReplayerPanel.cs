using System;
using System.Linq;
using System.Windows.Forms;
using lib.GraphImpl;
using lib.Scores.Simple;
using lib.StateImpl;
using lib.viz.Detalization;

namespace lib.viz
{
    public class ReplayerPanel : Panel
    {
        private readonly Timer timer;
        private readonly MapPainter mapPainter;
        private readonly ScorePanel scorePanel;
        private IReplayDataProvider provider;
        private FuturesPanel futuresPanel;

        public ReplayerPanel()
        {
            mapPainter = new MapPainter
            {
                PainterAugmentor = new DefaultPainterAugmentor {ShowFutures = true}
            };
            mapPanel = new ScaledViewPanel(mapPainter)
            {
                Dock = DockStyle.Fill
            };
            scorePanel = new ScorePanel {Dock = DockStyle.Top, Height = 40};
            scorePanel.PlayerSelected += PlayerSelected;

            futuresPanel = new FuturesPanel();
            futuresPanel.FuturesVisibleChanged += v => mapPainter.PainterAugmentor.ShowFutures = v;
            futuresPanel.FuturesVisibleChanged += _ => Refresh();

            var middlePanel = new TableLayoutPanel {Dock = DockStyle.Fill};
            middlePanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.8f});
            middlePanel.ColumnStyles.Add(new ColumnStyle {SizeType = SizeType.Percent, Width = 0.2f});
            middlePanel.Controls.Add(mapPanel, 0, 0);
            middlePanel.Controls.Add(futuresPanel, 1, 0);
            Controls.Add(middlePanel);
            Controls.Add(scorePanel);

            progressPanel = new ProgressControlPanel();
            progressPanel.CurrentStateUpdated += state => mapPainter.GameState = state;
            progressPanel.CurrentStateUpdated += _ => mapPanel.Refresh();
            progressPanel.CurrentStateUpdated += state => futuresPanel.UpdateFuturesStats(state.CurrentMap);
            Controls.Add(progressPanel);
            timer = new Timer {Interval = 500};
            timer.Tick += OnTick;
            timer.Start();
        }

        private void PlayerSelected(int playerIndex, string playerName)
        {
            mapPainter.PainterAugmentor.SelectedPlayerIndex = playerIndex;
            mapPanel.Invalidate();
        }

        private bool liveScoreUpdate;
        private ProgressControlPanel progressPanel;
        private ScaledViewPanel mapPanel;

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

            var playersCount = newProvider.PunterNames.Length;
            mapPainter.Futures = Enumerable.Range(0, playersCount).ToDictionary(i => i, newProvider.GetPunterFutures);
            futuresPanel.SetFutures(mapPainter.Futures, map);

            mapPainter.Map = map;
            mapPainter.GameState = null;
            scorePanel.SetPlayers(newProvider.PunterNames);
            Refresh();

            progressPanel.SetNextStateGenerator(newProvider.NextMove, newProvider is LogReplayDataProvider);
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
            scorePanel.SetScores(scores.ToArray(), gameState.SplurgePoints);
        }
    }
}