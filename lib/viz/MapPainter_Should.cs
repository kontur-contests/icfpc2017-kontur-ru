using System;
using System.IO;
using System.Windows.Forms;
using lib.Ai;
using lib.Structures;
using lib.viz.Detalization;
using NUnit.Framework;

namespace lib.viz
{
    [TestFixture]
    public class MapPainter_Should
    {
        [Test]
        [STAThread]
        [Explicit]
        public void Show()
        {
            var map = MapLoader.LoadMapByNameInTests("circle.json").Map;
            map = map.ApplyMove(Move.Claim(0, 0, 1));
            map = map.ApplyMove(Move.Option(1, 1, 0));
            map.Show();
        }
    }
}