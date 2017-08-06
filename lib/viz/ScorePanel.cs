using System;
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
        private FlowLayoutPanel layout;

        public event Action<int, string> PlayerSelected;

        public ScorePanel()
        {
            layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Height = 1,
                AutoSize = true,
            };
            layout.SizeChanged += (sender, args) => Resized();
            Controls.Add(layout);
        }

        private void Resized()
        {
            layout.PerformLayout();
            if (labels.Any())
                Height = layout.Height = labels.Last().Bottom - labels.First().Top;
            else
            {
                Height = 1;
            }
        }

        public void SetPlayers(IList<string> playerNames)
        {
            foreach (var label in labels)
                layout.Controls.Remove(label);
            labels.Clear();
            for (var index = 0; index < playerNames.Count; index++)
            {
                string player = playerNames[index];
                var label = new Label
                {
                    Text = FormatScore(player, 0, 0),
                    AutoSize = true,
                    Font = new Font(new FontFamily("Arial"), 12),
                    ForeColor = Color.White,
                    BackColor = ColorsPalette.Colors[index],
                    Padding = new Padding(0),
                    Margin = new Padding(0),
                    Tag = player
                };
                var indexCopy = index;
                label.MouseEnter += (sender, args) => PlayerSelected?.Invoke(indexCopy, player);
                label.MouseLeave += (sender, args) => PlayerSelected?.Invoke(-1, "");
                labels.Add(label);
                layout.Controls.Add(label);
            }
            Resized();
        }

        public void SetScores(long[] scores, long[] splurgePoints)
        {
            var bestScoreIndex = 0;
            for (var index = 0; index < scores.Length; index++)
            {
                var result = scores[index];
                labels[index].Text = FormatScore((string)labels[index].Tag, result, splurgePoints[index]);
                if (result > scores[bestScoreIndex])
                    bestScoreIndex = index;
            }
            for (var index = 0; index < scores.Length; index++)
            {
                labels[index].Font = new Font(labels[index].Font, index == bestScoreIndex ? FontStyle.Underline : FontStyle.Regular);
            }
            PerformLayout();
        }

        private static string FormatScore(string name, long score, long splurgePoints)
        {
            return $"{name.ToShortUpperLetters()}: {score}|{splurgePoints}";
        }
    }
}