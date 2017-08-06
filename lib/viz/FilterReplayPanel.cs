using System;
using System.Linq;
using System.Windows.Forms;
using lib.Replays;

namespace lib.viz
{
    internal class FilterReplayPanel : TableLayoutPanel
    {
        public FilterReplayPanel()
        {
            Dock = DockStyle.Top;

            aiFilter = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            aiFilter.SelectedIndexChanged += (_, __) => ApplyFilters();

            sizeFilter = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            sizeFilter.Items.Add("*");
            sizeFilter.Items.Add("SMALL");
            sizeFilter.Items.Add("MEDIUM");
            sizeFilter.Items.Add("BIG");
            sizeFilter.SelectedIndex = 0;
            sizeFilter.SelectedIndexChanged += (_, __) => ApplyFilters();

            winFilter = new CheckBox
            {
                Dock = DockStyle.Top,
                Text = "WINS",
                CheckState = CheckState.Checked
            };
            winFilter.CheckStateChanged += (_, __) => ApplyFilters();
            loseFilter = new CheckBox
            {
                Dock = DockStyle.Top,
                Text = "LOSES",
                CheckState = CheckState.Checked
            };
            loseFilter.CheckStateChanged += (_, __) => ApplyFilters();

            Controls.Add(aiFilter, 0, 0);
            Controls.Add(sizeFilter, 0, 1);
            Controls.Add(winFilter, 0, 2);
            Controls.Add(loseFilter, 1, 2);
        }

        public void UpdateMetas(ReplayMeta[] metas)
        {
            this.metas = metas;
            var selectedItem = aiFilter.SelectedItem;
            aiFilter.Items.Clear();
            aiFilter.Items.Add("*");
            aiFilter.Items.AddRange(metas.Select(m => $"{m.AiName}").Distinct().OrderBy(s => s).Cast<object>().ToArray());
            if (selectedItem != null && aiFilter.Items.Contains(selectedItem))
                aiFilter.SelectedItem = selectedItem;
            else
                aiFilter.SelectedIndex = 0; 

            ApplyFilters();
        }

        public event Action<ReplayMeta[]> FiltersUpdated;

        private void ApplyFilters()
        {
            FiltersUpdated?.Invoke(metas.Where(Captured).ToArray());
        }

        private bool Captured(ReplayMeta meta)
        {
            var ai = aiFilter.SelectedItem.ToString();

            var top = SizeGroups[sizeFilter.SelectedIndex].Item2;
            var bottom = SizeGroups[sizeFilter.SelectedIndex].Item1;

            if (!(meta.AiName == ai || ai == "*"))
                return false;
            if (!(meta.PunterCount >= bottom && meta.PunterCount <= top))
                return false;

            var ourScore = meta.Scores.First(s => s.punter == meta.OurPunter).score;
            var count = meta.Scores.Count(s => s.score < ourScore) + 1;
            var win = count == meta.Scores.Length;
            return (loseFilter.Checked || win) && (winFilter.Checked || !win);
        }

        private readonly ComboBox aiFilter;
        private ReplayMeta[] metas;
        private readonly ComboBox sizeFilter;
        private CheckBox winFilter;
        private CheckBox loseFilter;

        private static readonly Tuple<int, int>[] SizeGroups =
        {
            Tuple.Create(-1, int.MaxValue),
            Tuple.Create(2, 4),
            Tuple.Create(4, 8),
            Tuple.Create(8, int.MaxValue)
        };
    }
}