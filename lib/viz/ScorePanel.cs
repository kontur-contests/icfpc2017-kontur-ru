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

        public void SetPlayers(IList<string> playerNames)
        {
            foreach (var label in labels)
                Controls.Remove(label);
            labels.Clear();
            for (var index = 0; index < playerNames.Count; index++)
            {
                var player = playerNames[index];
                var label = new Label
                {
                    Text = player + ": 0",
                    Dock = DockStyle.Right,
                    AutoSize = true,
                    Font = new Font(new FontFamily("Arial"), 14),
                    ForeColor = Color.White,
                    BackColor = ColorsPalette.Colors[index],
                    Padding = new Padding(5),
                    Tag = player
                };
                labels.Add(label);
                Controls.Add(label);
            }
            PerformLayout();
            if (labels.Any())
                Height = labels[0].Height;
        }

        public void SetScores(long[] scores)
        {
            var bestScoreIndex = 0;
            for (var index = 0; index < scores.Length; index++)
            {
                var result = scores[index];
                labels[index].Text = FormatScore((string)labels[index].Tag, result);
                if (result > scores[bestScoreIndex])
                    bestScoreIndex = index;
            }
            for (var index = 0; index < scores.Length; index++)
            {
                labels[index].Font = new Font(labels[index].Font, index == bestScoreIndex ? FontStyle.Underline : FontStyle.Regular);
            }
            PerformLayout();
        }

        private static string FormatScore(string name, long score)
        {
            return name + ": " + score;
        }
    }
}