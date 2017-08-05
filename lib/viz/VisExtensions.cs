using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using lib.Ai;
using lib.viz;
using NUnit.Framework;

namespace lib
{
    public static class VisExtensions
    {
        public static void Visualize(this Map map)
        {
            var form = new Form();
            var painter = new MapPainter {Map = map};

            var panel = new ScaledViewPanel(painter)
            {
                Dock = DockStyle.Fill
            };
            form.Controls.Add(panel);
            form.ShowDialog();
        }
    }
}