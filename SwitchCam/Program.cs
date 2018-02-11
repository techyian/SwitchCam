using System;
using Gtk;

namespace SwitchCam
{
    public class Program
    {
        public static Application App;
        public static Window Win;

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            App = new Application("org.Samples.Samples", GLib.ApplicationFlags.None);
            App.Register(GLib.Cancellable.Current);

            Win = new ConfigForm();
            App.AddWindow(Win);

            var menu = new GLib.Menu();            
            menu.AppendItem(new GLib.MenuItem("Quit", "app.quit"));
            App.AppMenu = menu;
            
            var quitAction = new GLib.SimpleAction("quit", null);
            quitAction.Activated += QuitActivated;
            App.AddAction(quitAction);

            Win.ShowAll();
            Application.Run();
        }

        private static void QuitActivated(object sender, EventArgs e)
        {
            Application.Quit();
        }
    }
}
