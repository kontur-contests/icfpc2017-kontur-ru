using System;
using System.Collections.Generic;
using System.Windows.Forms;
using lib.GraphImpl;
using lib.Structures;

namespace lib.viz
{
    public class FuturesPanel : TableLayoutPanel
    { 
        public  void SetFutures(Dictionary<int, Future[]> futures, Map map)
        {
            this.futures = futures;
            var graph = new Graph(map);
            futureToListIndex.Clear();
            futuresList.Items.Clear();
            var calculator = new MineDistCalculator(graph);
            var i = 0;
            foreach (var futuresGroup in futures)
            {
                var id = futuresGroup.Key;
                foreach (var future in futuresGroup.Value)
                {
                    var str = $"{id}:\t{future.source}->{future.target}\t|{CalculateCost(future, calculator)}";
                    futuresList.Items.Add(str, false);
                    futureToListIndex[future] = i++;
                }
            }

            UpdateFuturesStats(map);
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
            futuresList = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.None
            };

            Controls.Add(show, 0, 0);
            Controls.Add(futuresList, 0, 1);
        }

        public void UpdateFuturesStats(Map map)
        {
            var graph = new Graph(map);
            foreach (var futuresGroup in futures)
            {
                var connectedCalc = new ConnectedCalculator(graph, futuresGroup.Key);
                foreach (var future in futuresGroup.Value)
                {
                    var connected = connectedCalc.GetConnectedMines(future.source).Contains(future.target) ||
                                    connectedCalc.GetConnectedMines(future.target).Contains(future.source);

                    futuresList.SetItemChecked(futureToListIndex[future], connected);
                }
            }
        }

        private static int CalculateCost(Future future, MineDistCalculator calculator)
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

        private readonly CheckedListBox futuresList;
        private Dictionary<int, Future[]> futures;
        private Dictionary<Future, int> futureToListIndex = new Dictionary<Future, int>();
    }
}