using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using lib.Replays;
using MoreLinq;

namespace lib.viz
{
    internal class SelectReplayPanel : Panel
    {
        public SelectReplayPanel()
        {
            debugTextArea = new TextBox()
            {
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Consolas", 12),
                Dock = DockStyle.Bottom,
                Text = "",
                Height = 400,
                ScrollBars = ScrollBars.Vertical
            };
            listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false,
                Width = 600
            };

            listView.FullRowSelect = true;
            listView.HideSelection = false;

            listView.Columns.Add("Time");
            listView.Columns.Add("Ai");
            listView.Columns.Add("Points per game (max = PuntersCount)");
            listView.Columns.Add("PuntersCount");


            listView.ItemSelectionChanged += SelectedReplayChanged;
            Controls.Add(debugTextArea);
            Controls.Add(listView);
        }

        private void SelectedReplayChanged(object sender, ListViewItemSelectionChangedEventArgs args)
        {
            if (repo == null || listView.SelectedItems.Count == 0) return;
            var lvItem = listView.SelectedItems[0];
            var meta = (ReplayMeta)lvItem.Tag;
            var data = repo.GetData(meta.DataId);
            debugTextArea.Text = meta.ToString() + "\r\n\r\n" + data.Moves.ToDelimitedString("\r\n");
            SelectedReplay = new ReplayFullData(meta, data);
            ReplayChanged?.Invoke();
        }

        private ReplayRepo repo;
        private ListView listView;
        private TextBox debugTextArea;

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
                var lvItem = listView.Items.Add(meta.Timestamp.ToString("T"));
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
}