using System;
using System.Threading.Tasks;
using Gtk;
using MMALSharp;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using Raspberry.IO.GeneralPurpose;

namespace MakeACameraWithPiZero
{
    public class ConfigForm : Window
    {
        const ConnectorPin buttonPin = ConnectorPin.P1Pin22;

        /// <summary> Used to load in the glade file resource as a window. </summary>
        private Builder _builder;

        [Builder.Object]
        private ComboBox _sharpnessCombo;

        [Builder.Object]
        private ComboBox _contrastCombo;

        [Builder.Object]
        private ComboBox _brightnessCombo;

        [Builder.Object]
        private ComboBox _saturationCombo;

        [Builder.Object]
        private ComboBox _isoCombo;

        [Builder.Object]
        private ComboBox _effectsCombo;

        [Builder.Object]
        private ComboBox _imageSizeCombo;

        private GpioConnection _buttonConnection;

        public MMALCamera MMALCamera = MMALCamera.Instance;

        public bool ReloadConfig { get; set; }

        public static ConfigForm Create()
        {
            Builder builder = new Builder(null, "MakeACameraWithPiZero.MakeACameraWithPiZero.glade", null);
            return new ConfigForm(builder, builder.GetObject("ConfigWindow").Handle);
        }

        /// <summary>Specialised constructor for use only by derived class.</summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="handle">  The handle. </param>
        protected ConfigForm(Builder builder, IntPtr handle) : base(handle)
        {
            Application.Init();

            this._builder = builder;
            builder.Autoconnect(this);

            this.ConfigureButton();
            this.InitialiseComboBoxes();
            this.SetupHandlers();

            Application.Run();
        }

        private void ConfigureButton()
        {
            var switchButton = buttonPin.Input()
                  .Name("Switch")
                  .Revert()
                  .Switch()
                  .Enable()
                  .OnStatusChanged(b =>
                  {
                      Console.WriteLine("Button switched {0}", b ? "on" : "off");

                      AsyncContext.Run(async () =>
                      {
                          using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))
                          using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                          using (var nullSink = new MMALNullSinkComponent())
                          {
                              this.MMALCamera.ConfigureCameraSettings();

                              // Create our component pipeline.
                              imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                              this.MMALCamera.Camera.StillPort.ConnectTo(imgEncoder);
                              this.MMALCamera.Camera.PreviewPort.ConnectTo(nullSink);

                              // Camera warm up time
                              await Task.Delay(2000);
                              await this.MMALCamera.BeginProcessing(this.MMALCamera.Camera.StillPort);
                          }
                      });
                  });

            this._buttonConnection = new GpioConnection(switchButton);
        }

        private void InitialiseComboBoxes()
        {
            // Sharpness ComboBox
            var sharpnessModel = new ListStore(typeof(int),
                                               typeof(string));

            this._sharpnessCombo.Model = sharpnessModel;

            for (int i = 0; i <= 10; i++)
            {
                sharpnessModel.AppendValues(i * 10, (i * 10).ToString());
            }

            // Set the default value for this combo box
            this._sharpnessCombo.Active = 4;

            // Contrast ComboBox
            var contrastModel = new ListStore(typeof(int),
                                              typeof(string));

            this._contrastCombo.Model = contrastModel;

            for (int i = 0; i <= 10; i++)
            {
                contrastModel.AppendValues(i * 10, (i * 10).ToString());
            }

            this._contrastCombo.Active = 4;

            // Brightness ComboBox
            var brightnessModel = new ListStore(typeof(int),
                                                typeof(string));

            this._brightnessCombo.Model = brightnessModel;

            for (int i = 0; i <= 10; i++)
            {
                brightnessModel.AppendValues(i * 10, (i * 10).ToString());
            }

            this._brightnessCombo.Active = 4;

            // Saturation ComboBox
            var saturationModel = new ListStore(typeof(int),
                                                typeof(string));

            this._saturationCombo.Model = saturationModel;

            for (int i = 0; i <= 10; i++)
            {
                saturationModel.AppendValues(i * 10, (i * 10).ToString());
            }

            this._saturationCombo.Active = 4;

            // ISO ComboBox
            var isoModel = new ListStore(typeof(int),
                                         typeof(string));

            this._isoCombo.Model = isoModel;

            isoModel.AppendValues(100, "100");
            isoModel.AppendValues(200, "200");
            isoModel.AppendValues(400, "400");
            isoModel.AppendValues(800, "800");

            this._isoCombo.Active = 0;

            // Effects ComboBox
            var effectsModel = new ListStore(typeof(int),
                                             typeof(string));

            this._effectsCombo.Model = effectsModel;

            foreach (MMAL_PARAM_IMAGEFX_T effect in Enum.GetValues(typeof(MMAL_PARAM_IMAGEFX_T)))
            {
                effectsModel.AppendValues(effect, effect.ToString());
            }

            // Image size ComboBox
            var imageSizeModel = new ListStore(typeof(string),
                                               typeof(string));

            this._imageSizeCombo.Model = imageSizeModel;

            imageSizeModel.AppendValues("3264,2448", "8 Megapixel");
            imageSizeModel.AppendValues("3072,2304", "7 Megapixel");
            imageSizeModel.AppendValues("3032,2008", "6 Megapixel");
            imageSizeModel.AppendValues("2560,1920", "5 Megapixel");
            imageSizeModel.AppendValues("2240,1680", "4 Megapixel");
            imageSizeModel.AppendValues("2048,1536", "3 Megapixel");
            imageSizeModel.AppendValues("1600,1200", "2 Megapixel");
            imageSizeModel.AppendValues("1280,960", "1 Megapixel");
            imageSizeModel.AppendValues("640,480", "0.3 Megapixel");

            this._imageSizeCombo.Active = 0;
        }

        /// <summary> Sets up the handlers. </summary>
        private void SetupHandlers()
        {
            this.DeleteEvent += new DeleteEventHandler(this.OnDestroy);
            this._sharpnessCombo.Changed += new EventHandler(this.OnSharpnessChanged);
            this._contrastCombo.Changed += new EventHandler(this.OnContrastChanged);
            this._brightnessCombo.Changed += new EventHandler(this.OnBrightnessChanged);
            this._saturationCombo.Changed += new EventHandler(this.OnSaturationChanged);
            this._isoCombo.Changed += new EventHandler(this.OnISOChanged);
            this._effectsCombo.Changed += new EventHandler(this.OnEffectChanged);
            this._imageSizeCombo.Changed += new EventHandler(this.OnImageSizeChanged);
        }

        public void OnSharpnessChanged(object o, EventArgs args)
        {
            this.ReloadConfig = true;
        }

        public void OnBrightnessChanged(object o, EventArgs args)
        {
            this.ReloadConfig = true;
        }

        public void OnContrastChanged(object o, EventArgs args)
        {
            this.ReloadConfig = true;
        }

        public void OnSaturationChanged(object o, EventArgs args)
        {
            this.ReloadConfig = true;
        }

        public void OnISOChanged(object o, EventArgs args)
        {
            this.ReloadConfig = true;
        }

        public void OnEffectChanged(object o, EventArgs args)
        {
            this.ReloadConfig = true;
        }

        public void OnImageSizeChanged(object o, EventArgs args)
        {
            this.ReloadConfig = true;
        }

        public void OnDestroy(object o, DeleteEventArgs args)
        {
            Console.WriteLine("OnDestroy");
            this.MMALCamera.Cleanup();
            this._buttonConnection.Close();
            Application.Quit();
        }
    }
}
