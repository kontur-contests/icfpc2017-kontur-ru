using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using lib.GraphImpl;
using lib.Structures;

namespace lib.viz
{
    public class FuturesPanel : TableLayoutPanel
    { 
        public  void SetFutures(Dictionary<int, Future[]> futures, Graph graph)
        {
            var calculator = new MineDistCalculator.Impl(graph);
            futuresList.Items.Clear();
            foreach (var futuresGroup in futures)
            {
                var id = futuresGroup.Key;
                futuresList.Items.AddRange(futuresGroup.Value.Select(f => $"{id}:\t{f.source}->{f.target}\t|{CalculateCost(f, calculator)}").Cast<object>().ToArray());
            }
        }

        public event Action<bool> FuturesVisibleChanged;

        public FuturesPanel()
        {
            Dock = DockStyle.Fill;

            var show = new CheckBox
            {
                Text = "SHOW FUTURES",
                Dock = DockStyle.Top,
                CheckState = CheckState.Checked
            };
            show.CheckStateChanged += (_, __) => FuturesVisibleChanged?.Invoke(show.Checked);
            futuresList = new ListBox
            {
                Dock = DockStyle.Fill
            };

            RowStyles.Add(new RowStyle { SizeType = SizeType.Absolute, Height = 40 });
            Controls.Add(new Button(), 0, 0);
            Controls.Add(show, 0, 2);
            Controls.Add(futuresList, 0, 3);
        }

        private static int CalculateCost(Future future, MineDistCalculator.Impl calculator)
        {
            var dist = 0;

            try
            {
                dist = calculator.GetDist(future.source, future.target);
            }
            catch (InvalidOperationException)
            {
                try
                {
                    dist = calculator.GetDist(future.target, future.source);
                }
                catch (InvalidOperationException)
                {
                    return 0;
                }
            }

            return dist * dist * dist;
        }

        private readonly ListBox futuresList;
    }
}