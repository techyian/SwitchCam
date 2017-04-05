using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gtk;

namespace MakeACameraWithPiZero
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Init();            
            ConfigForm form = ConfigForm.Create();
            form.Show();
            Application.Run();
        }
    }
}
