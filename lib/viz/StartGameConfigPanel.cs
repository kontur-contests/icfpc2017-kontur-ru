using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using lib.Ai;
using lib.StateImpl;
using lib.Structures;
using lib.viz;
using NUnit.Framework;

namespace lib
{
    internal class StartGameConfigPanel : Panel
    {
        private readonly ListBox allAisList;
        private readonly ListBox mapsList;
        private ListBox selectedAisList;

        public StartGameConfigPanel()
        {
            var mapsListLabel = new Label
            {
                Dock = DockStyle.Top,
                Text = "Maps list. Click to select."
            };
            mapsList = new ListBox
            {
                Dock = DockStyle.Fill
            };
            mapsList.SelectedValueChanged += (sender, args) =>
            {
                if (mapsList.SelectedItem != null)
                    MapChanged?.Invoke((NamedMap) mapsList.SelectedItem);
            };
            var allAisListLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Text = "AIs list. Double click to add AI."
            };
            allAisList = new ListBox
            {
                Dock = DockStyle.Bottom,
                Height = 200
            };
            allAisList.DoubleClick += (sender, args) =>
            {
                if (allAisList.SelectedItem != null)
                    AiSelected?.Invoke((AiFactory) allAisList.SelectedItem);
            };
            var selectedAisListLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Text = "Selected AIs list. Double click to remove."
            };
            selectedAisList = new ListBox
            {
                Dock = DockStyle.Bottom,
                Height = 100
            };
            var enableFutures = new CheckBox
            {
                Text = "ENABLE FUTURES",
                Dock = DockStyle.Bottom,
                CheckState = CheckState.Checked
            };
            var enablSplurges = new CheckBox
            {
                Text = "ENABLE SPLURGES",
                Dock = DockStyle.Bottom,
                CheckState = CheckState.Checked
            };
            var enablOptions = new CheckBox
            {
                Text = "ENABLE OPTIONS",
                Dock = DockStyle.Bottom,
                CheckState = CheckState.Checked
            };
            enableFutures.CheckStateChanged += (sender, args) =>
            {
                EnableFuturesChanged?.Invoke(enableFutures.Checked);
            };
            enablSplurges.CheckStateChanged += (sender, args) =>
            {
                EnableSplurgesChanged?.Invoke(enablSplurges.Checked);
            };
            enablOptions.CheckStateChanged += (sender, args) =>
            {
                EnableOptionsChanged?.Invoke(enablOptions.Checked);
            };
            selectedAisList.DoubleClick += (sender, args) =>
            {
                AiAtIndexRemoved?.Invoke(selectedAisList.SelectedIndex);
            };
            EnableFuturesChanged += enable =>
            {
                Settings.futures = enable;
            };
            EnableSplurgesChanged += enable =>
            {
                Settings.splurges = enable;
            };
            EnableOptionsChanged += enable =>
            {
                Settings.options = enable;
            };
            AiSelected += factory =>
            {
                var ai = factory.Create();
                SelectedAis.Add(ai);
                selectedAisList.Items.Add($"{ai.Name}:{ai.Version}");
            };
            AiAtIndexRemoved += index =>
            {
                selectedAisList.Items.RemoveAt(index);
                SelectedAis.RemoveAt(index);
            };
            MapChanged += map => { SelectedMap = map; };

            var fastAiSelectors = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true
            };
            for (var i = 1; i < 5; i++)
            {
                var cnt = (int)Math.Pow(2, i);
                var button = new Button
                {
                    Text = cnt.ToString(),
                    Dock = DockStyle.Left,
                    Width = 30
                };
                button.Click += (_, __) => AddRandomAis(cnt);
                fastAiSelectors.Controls.Add(button, i, 0);
            }

            Controls.Add(mapsList);
            Controls.Add(mapsListLabel);
            Controls.Add(allAisListLabel);
            Controls.Add(fastAiSelectors);
            Controls.Add(allAisList);
            Controls.Add(selectedAisListLabel);
            Controls.Add(selectedAisList);
            Controls.Add(enableFutures);
            Controls.Add(enablSplurges);
            Controls.Add(enablOptions);
        }

        private void ClearSelected()
        {
            for (int i = selectedAisList.Items.Count - 1; i >= 0; i--)
                AiAtIndexRemoved?.Invoke(i);
            selectedAisList.Items.Clear();
        }

        public List<IAi> SelectedAis { get; } = new List<IAi>();
        public NamedMap SelectedMap { get; private set; }

        public Settings Settings { get; private set; } = new Settings(true, true, true);

        public event Action<NamedMap> MapChanged;
        public event Action<AiFactory> AiSelected;
        public event Action<int> AiAtIndexRemoved;
        public event Action<bool> EnableFuturesChanged;
        public event Action<bool> EnableSplurgesChanged;
        public event Action<bool> EnableOptionsChanged;

        public void SetMaps(NamedMap[] maps)
        {
            mapsList.Items.AddRange(maps.Cast<object>().ToArray());
            mapsList.SelectedIndex = 0;
        }

        public void SetAis(params AiFactory[] ais)
        {
            allAisList.Items.AddRange(ais.Cast<object>().ToArray());
        }

        private void AddRandomAis(int count)
        {
            ClearSelected();
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                AiSelected?.Invoke((AiFactory)allAisList.Items[random.Next(0, allAisList.Items.Count)]);
            }
        }
    }

    public class StartGameConfigPanel_Tests
    {
        [Explicit]
        [Test]
        [STAThread]
        public void Show()
        {
            var form = new Form()
            {
                Size = new Size(300, 800)
            };
            var panel = new StartGameConfigPanel
            {
                Dock = DockStyle.Fill
            };
            panel.SetMaps(MapLoader.LoadDefaultMaps().ToArray());
            panel.SetAis(new AiFactory("Basic", () => new JunkAi()));
            panel.MapChanged += map => form.Text = map.Name;
            panel.AiSelected += factory => form.Text = factory.Name;
            form.Controls.Add(panel);
            form.ShowDialog();
        }

        [ShouldNotRunOnline(DisableCompletely = true)]
        private class JunkAi : IAi
        {
            public string Name => "Junk";
            public string Version { get; }

            public AiSetupDecision Setup(State state, IServices services)
            {
                throw new NotSupportedException();
            }

            public AiMoveDecision GetNextMove(State state, IServices services)
            {
                throw new NotSupportedException();
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}