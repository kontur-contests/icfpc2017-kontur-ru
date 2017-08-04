using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using lib.viz.Detalization;
using MoreLinq;

namespace lib.viz
{
    public class ScorePanel : Panel
    {
        private List<Label> labels = new List<Label>();

        public ScorePanel()
        {
        }

        public void SetPlayers(IList<IAi> players)
        {
            foreach (var label in labels)
                Controls.Remove(label);
            labels.Clear();
            for (var index = 0; index < players.Count; index++)
            {
                var player = players[index];
                var label = new Label
                {
                    Text = player.Name + ": 0",
                    Dock = DockStyle.Right,
                    AutoSize = true,
                    Font = new Font(new FontFamily("Arial"), 14),
                    ForeColor = Color.White,
                    BackColor = ColorsPalette.Colors[index],
                    Padding = new Padding(5)
                };
                labels.Add(label);
                Controls.Add(label);
            }
            PerformLayout();
            if (labels.Any())
                Height = labels[0].Height;
        }

        public void SetScores(GameSimulationResult[] results)
        {
            var bestScoreIndex = 0;
            for (var index = 0; index < results.Length; index++)
            {
                var result = results[index];
                labels[index].Text = FormatScore(result);
                if (result.Score > results[bestScoreIndex].Score)
                    bestScoreIndex = index;
            }
            for (var index = 0; index < results.Length; index++)
            {
                labels[index].Font = new Font(labels[index].Font, index == bestScoreIndex ? FontStyle.Underline : FontStyle.Regular);
            }
            PerformLayout();
        }

        private static string FormatScore(GameSimulationResult result)
        {
            return result.Gamer.Name + ": " + result.Score;
        }
    }
}