using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gtk;
using MMALSharp;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using Unosquare.RaspberryIO.Peripherals;

namespace SwitchCam
{
    public class ConfigForm : Window
    {
        private Unosquare.RaspberryIO.Gpio.GpioPin _buttonPin = Pi.Gpio.GetGpioPinByBcmPinNumber(23);

        private Unosquare.RaspberryIO.Peripherals.Button _button;
        private HeaderBar _headerBar;
        private TreeView _treeView;
        private Box _boxContent;
        private TreeStore _store;
        private Dictionary<string, Tuple<Type, Widget>> _items;
        private Notebook _notebook;

        public MMALCamera MMALCamera = MMALCamera.Instance;

        public static bool ReloadConfig { get; set; }
        public static bool Processing { get; set; }

        public ConfigForm() : base(WindowType.Toplevel)
        {
            // Setup GUI
            WindowPosition = WindowPosition.Center;
            DefaultSize = new Gdk.Size(500, 350);

            _headerBar = new HeaderBar();
            _headerBar.ShowCloseButton = true;
            _headerBar.Title = "SwitchCam";

            var btnClickMe = new Gtk.Button();
            btnClickMe.AlwaysShowImage = true;
            btnClickMe.Image = Image.NewFromIconName("document-new-symbolic", IconSize.Button);
            _headerBar.PackStart(btnClickMe);

            Titlebar = _headerBar;

            var vpanned1 = new VPaned();
            vpanned1.Position = 300;
            
            var hpanned = new HPaned();
            hpanned.Position = 100;

            _treeView = new TreeView();
            _treeView.HeadersVisible = false;
            hpanned.Pack1(_treeView, false, true);

            _notebook = new Notebook();

            var scroll1 = new ScrolledWindow();
            var vpanned = new VPaned();
            vpanned.Position = 300;
            _boxContent = new Box(Orientation.Vertical, 0);
            _boxContent.Margin = 8;
            vpanned.Pack1(_boxContent, false, false);
            scroll1.Child = vpanned;
            _notebook.AppendPage(scroll1, new Label { Text = "Data", Expand = true });

            hpanned.Pack2(_notebook, false, true);

            vpanned1.Pack1(hpanned, false, true);

            var box = new Box(Orientation.Horizontal, 0);
            box.Margin = 8;
            vpanned1.Pack2(box, false, true);

            var grid = new Grid
            {
                RowSpacing = 2,
                ColumnSpacing = 2
            };

            box.PackStart(grid, false, false, 0);

            var btn = new Gtk.Button("Take Picture");
            btn.Clicked += TakePicture;

            grid.Attach(btn, 0, 0, 1, 1);

            Child = vpanned1;

            // Fill up data
            FillUpTreeView();

            // Connect events
            _treeView.Selection.Changed += Selection_Changed;
            Destroyed += OnDestroy;

            try
            {
                ConfigureButton();
            }
            catch (Exception e)
            {
                MMALLog.Logger.Debug($"Something went wrong while configuring the button. {e.Message} {e.StackTrace}");
            }
        }

        private void ConfigureButton()
        {
            _buttonPin.PinMode = GpioPinDriveMode.Input;
            _button = new Unosquare.RaspberryIO.Peripherals.Button(_buttonPin);
            _buttonPin.InputPullMode = GpioPinResistorPullMode.PullUp;

            MMALLog.Logger.Debug($"Input button configured");

            _button.Released += (s, e) =>
            {
                MMALLog.Logger.Debug($"Button released");
            };

            _button.Pressed += (s, e) =>
            {
                MMALLog.Logger.Debug($"Button pressed");

                if (Processing)
                {
                    return;
                }

                Processing = true;

                if (ReloadConfig)
                {
                    this.MMALCamera.ConfigureCameraSettings();
                    ConfigForm.ReloadConfig = false;
                }

                AsyncContext.Run(async () =>
                {
                    using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))
                    using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                    using (var renderer = new MMALVideoRenderer())
                    {
                        this.MMALCamera.ConfigureCameraSettings();

                        // Create our component pipeline.
                        imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                        this.MMALCamera.Camera.StillPort.ConnectTo(imgEncoder);
                        this.MMALCamera.Camera.PreviewPort.ConnectTo(renderer);

                        await Task.Delay(2000);
                        await this.MMALCamera.BeginProcessing(this.MMALCamera.Camera.StillPort);
                        Processing = false;
                    }
                });
            };
        }

        private void Selection_Changed(object sender, EventArgs e)
        {
            if (_treeView.Selection.GetSelected(out TreeIter iter))
            {
                var s = _store.GetValue(iter, 0).ToString();

                while (_boxContent.Children.Length > 0)
                    _boxContent.Remove(_boxContent.Children[0]);
                _notebook.CurrentPage = 0;
                _notebook.ShowTabs = false;

                if (_items.TryGetValue(s, out var item))
                {
                    _notebook.ShowTabs = true;

                    if (item.Item2 == null)
                        _items[s] = item = new Tuple<System.Type, Widget>(item.Item1, Activator.CreateInstance(item.Item1) as Widget);

                    _boxContent.PackStart(item.Item2, true, true, 0);
                    _boxContent.ShowAll();
                }

            }
        }

        private void FillUpTreeView()
        {
            // Init cells
            var cellName = new CellRendererText();

            // Init columns
            var columeSections = new TreeViewColumn();
            columeSections.Title = "Sections";
            columeSections.PackStart(cellName, true);

            columeSections.AddAttribute(cellName, "text", 0);

            _treeView.AppendColumn(columeSections);

            // Init treeview
            _store = new TreeStore(typeof(string));
            _treeView.Model = _store;

            // Setup category base
            var dict = new Dictionary<Category, TreeIter>();
            foreach (var category in Enum.GetValues(typeof(Category)))
                dict[(Category)category] = _store.AppendValues(category.ToString());

            // Fill up categories
            _items = new Dictionary<string, Tuple<Type, Widget>>();
            var maintype = typeof(SectionAttribute);

            foreach (var type in maintype.Assembly.GetTypes())
            {
                foreach (var attribute in type.GetCustomAttributes(true))
                {
                    if (attribute is SectionAttribute a)
                    {
                        _store.AppendValues(dict[a.Category], a.Description);
                        _items[a.Description] = new Tuple<System.Type, Widget>(type, null);
                    }
                }
            }

            _treeView.ExpandAll();
        }

        private void TakePicture(object sender, EventArgs e)
        {
            if (ReloadConfig)
            {
                this.MMALCamera.ConfigureCameraSettings();
                ConfigForm.ReloadConfig = false;
            }

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))
            using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
            using (var renderer = new MMALVideoRenderer())
            {
                this.MMALCamera.ConfigureCameraSettings();

                // Create our component pipeline.
                imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                this.MMALCamera.Camera.StillPort.ConnectTo(imgEncoder);
                this.MMALCamera.Camera.PreviewPort.ConnectTo(renderer);

                Task.Factory.Run(async () =>
                {
                    // Camera warm up time
                    await Task.Delay(5000);
                    await this.MMALCamera.BeginProcessing(this.MMALCamera.Camera.StillPort);
                });
            }
            
        }

        public void OnDestroy(object o, EventArgs args)
        {
            Console.WriteLine("OnDestroy");
            this.MMALCamera.Cleanup();
            Pi.Gpio.Dispose();
            Application.Quit();
        }
    }
}
