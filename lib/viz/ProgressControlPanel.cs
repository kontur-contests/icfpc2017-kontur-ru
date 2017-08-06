using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace lib.viz
{
    public class ProgressControlPanel : TableLayoutPanel
    {
        public event Action<GameState> CurrentStateUpdated;

        public ProgressControlPanel()
        {
            Dock = DockStyle.Bottom;
            var revertStepButton = new Button
            {
                Dock = DockStyle.Fill,
                Text = "REVERT STEP"
            };
            gameProgress = new TrackBar
            {
                Dock = DockStyle.Top,
                Maximum = 0,
                Minimum = 0
            };
            var makeStepButton = new Button
            {
                Dock = DockStyle.Fill,
                Text = "MAKE STEP"
            };
            var autoPlay = new CheckBox
            {
                Dock = DockStyle.Bottom,
                Text = "AUTOPLAY",
                CheckState = CheckState.Unchecked
            };

            var middlePanel = new TableLayoutPanel {Dock = DockStyle.Fill};
            middlePanel.Controls.Add(gameProgress, 0, 0);
            middlePanel.SetColumnSpan(gameProgress, 2);
            middlePanel.Controls.Add(autoPlay, 0, 1);

            var timer = new Timer {Interval = delays[4]};
            timer.Tick += (_, __) => OnProgressUpdate(current + 1);
            autoPlay.CheckStateChanged += (_, __) => timer.Enabled = autoPlay.Checked;

            var speedControl = new TrackBar
            {
                Dock = DockStyle.Bottom,
                Maximum = delays.Length - 1,
                Minimum = 0,
                Value = 4
            };
            speedControl.ValueChanged += (_, __) => timer.Interval = delays[speedControl.Value];
            middlePanel.Controls.Add(speedControl, 1, 1);

            makeStepButton.Click += (_, __) => OnProgressUpdate(current + 1);
            gameProgress.ValueChanged += (_, __) => OnProgressUpdate(gameProgress.Value);
            revertStepButton.Click += (_, __) => OnProgressUpdate(current - 1);

            ColumnStyles.Add(new ColumnStyle { SizeType = SizeType.Percent, Width = 0.2f });
            ColumnStyles.Add(new ColumnStyle { SizeType = SizeType.Percent, Width = 0.6f });
            ColumnStyles.Add(new ColumnStyle { SizeType = SizeType.Percent, Width = 0.2f });
            Controls.Add(revertStepButton, 0, 0);
            Controls.Add(middlePanel, 1, 0);
            Controls.Add(makeStepButton, 2, 0);
        }

        public void SetNextStateGenerator(Func<GameState> generator, bool isReplay)
        {
            states.Clear();
            current = -1;
            nextStateGenerator = generator;
            gameProgress.Value = 0;
            gameProgress.Maximum = 0;

            if (isReplay)
                while (TryGenerateNextState())
                { }
        }

        private void OnProgressUpdate(int newCurrent)
        {
            if (newCurrent >= states.Count)
                if (!TryGenerateNextState())
                    return;
            if (newCurrent < 0 || newCurrent >= states.Count)
                return;
            current = newCurrent;
            gameProgress.Value = current;
            CurrentStateUpdated?.Invoke(states[current]);
        }

        private bool TryGenerateNextState()
        {
            if (nextStateGenerator == null)
                return false;
            if (states.Count != 0 && Enumerable.Last<GameState>(states).IsGameOver)
                return false;
            states.Add(nextStateGenerator());
            gameProgress.Maximum = states.Count - 1;
            return true;
        }

        private int current = -1;
        private readonly List<GameState> states = new List<GameState>();
        private readonly TrackBar gameProgress;
        private Func<GameState> nextStateGenerator;
        private static int[] delays = { 5000, 2000, 1000, 500, 250, 100, 50, 10 };
    }
}