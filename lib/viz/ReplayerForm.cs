using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using lib.Replays;
using NUnit.Framework;

namespace lib.viz
{
    public class ReplayerForm : Form
    {
        private readonly SelectReplayPanel selectReplayPanel;

        public ReplayerForm(ReplayRepo repo = null)
        {
            Size = new Size(800, 600);
            selectReplayPanel = new SelectReplayPanel
            {
                Dock = DockStyle.Left,
                Repo = repo
            };

            var rightPanel = new ReplayerPanel()
            {
                Dock = DockStyle.Fill
            };
            //UpdateMap(rightPanel);
            selectReplayPanel.ReplayChanged += () =>
            {
                UpdateMap(rightPanel);
            };

            Controls.Add(rightPanel);
            Controls.Add(selectReplayPanel);
        }

        private void UpdateMap(ReplayerPanel rightPanel)
        {
            rightPanel.SetDataProvider(selectReplayPanel.SelectedReplay.Data.Map, new LogReplayDataProvider(selectReplayPanel.SelectedReplay));
        }
    }

    internal class LogReplayDataProvider : IReplayDataProvider
    {
        private readonly ReplayFullData data;
        private readonly List<Move> prevMoves = new List<Move>();
        private int nextMoveIndex;
        private Map map;

        public LogReplayDataProvider(ReplayFullData data)
        {
            this.data = data;
            map = data.Data.Map;
            PunterNames = data.Meta.Scores
                .Select((s, i) => i == data.Meta.OurPunter ? data.Meta.AiName : i.ToString())
                .ToArray();
        }

        public string[] PunterNames { get; }
        public GameState NextMove()
        {
            var move = data.Data.Moves[nextMoveIndex++].ToMove();
            map = move.Execute(map);
            prevMoves.Add(move);
            return new GameState(map, move.PunterId, prevMoves, nextMoveIndex >= data.Data.Moves.Length);
        }
    }

    public class ReplayFullData
    {
        public ReplayFullData(ReplayMeta meta, ReplayData data)
        {
            Meta = meta;
            Data = data;
        }

        public ReplayMeta Meta;
        public ReplayData Data;
    }
    internal class SelectReplayPanel : Panel
    {
        public SelectReplayPanel()
        {
            listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false
            };

            listView.Width = 400;
            listView.Columns.Add("Time");
            listView.Columns.Add("Ai");
            listView.Columns.Add("W");
            listView.Columns.Add("N");

            listView.ItemSelectionChanged += SelectedReplayChanged;
            Controls.Add(listView);
        }

        private void SelectedReplayChanged(object sender, ListViewItemSelectionChangedEventArgs args)
        {
            if (repo == null || listView.SelectedItems.Count == 0) return;
            var lvItem = listView.SelectedItems[0];
            var meta = (ReplayMeta) lvItem.Tag;
            var data = repo.GetData(meta.DataId);
            SelectedReplay = new ReplayFullData(meta, data);
            ReplayChanged?.Invoke();
        }

        private ReplayRepo repo;
        private ListView listView;

        public ReplayRepo Repo
        {
            get { return repo; }
            set
            {
                repo = value;
                var metas = repo.GetRecentMetas();
                UpdateList(metas);
            }
        }

        private void UpdateList(ReplayMeta[] metas)
        {
            foreach (var meta in metas)
            {
                var lvItem = listView.Items.Add(meta.Timestamp.ToString());
                lvItem.Tag = meta;
                lvItem.SubItems.Add(meta.AiName);
                var ourScore = meta.Scores.First(s => s.Punter == meta.OurPunter).Score;
                var count = meta.Scores.Count(s => s.Score < ourScore);
                lvItem.SubItems.Add(count.ToString());
                lvItem.SubItems.Add(meta.Scores.Length.ToString());
            }
        }

        public ReplayFullData SelectedReplay { get; private set; }

        public event Action ReplayChanged;
    }

    [TestFixture]
    public class ReplayForm_Test
    {
        [Explicit]
        [Test]
        [STAThread]
        public void Test()
        {
            var repo = new ReplayRepo(true);
            var form = new ReplayerForm(repo);
            form.ShowDialog();
        }
    }
}