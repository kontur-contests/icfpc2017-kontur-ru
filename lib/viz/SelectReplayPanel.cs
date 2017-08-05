using System;
using System.Linq;
using System.Windows.Forms;
using lib.Replays;

namespace lib.viz
{
    internal class SelectReplayPanel : Panel
    {
        public SelectReplayPanel()
        {
            listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false,
                Width = 600
            };

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
}