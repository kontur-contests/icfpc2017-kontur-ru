using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using lib.Replays;
using lib.viz;

namespace VizReplaysApp
{
    static class VizReplaysAppProgram
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var replayRepo = new ReplayRepo();
                var form = new ReplayerForm(replayRepo);
                Application.Run(form);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Environment.Exit(-1);
            }
        }
    }
}
