using System;
using lib.Replays;
using NUnit.Framework;

namespace lib.viz
{
    [TestFixture]
    public class ReplayerForm_Test
    {
        [Explicit]
        [Test]
        [STAThread]
        public void Test()
        {
            var repo = new ReplayRepo();
            var form = new ReplayerForm(repo);
            form.ShowDialog();
        }
    }
}