using Gtk;
using MMALSharp;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MakeACameraWithPiZero
{
    public class ConfigForm : Window
    {
        /// <summary> Used to load in the glade file resource as a window. </summary>
        private Builder _builder;

        [Builder.Object]
        private ComboBox SharpnessCombo;

        [Builder.Object]
        private ComboBox ContrastCombo;

        [Builder.Object]
        private ComboBox BrightnessCombo;

        [Builder.Object]
        private ComboBox SaturationCombo;

        [Builder.Object]
        private ComboBox ISOCombo;

        [Builder.Object]
        private ComboBox EffectsCombo;

        [Builder.Object]
        private ComboBox ImageSizeCombo;

        public GpioConnection ButtonConnection { get; set; }

        const ConnectorPin buttonPin = ConnectorPin.P1Pin29;


        public MMALCamera Camera = MMALCamera.Instance;

        public ImageStreamCaptureHandler Handler { get; set; }

        public MMALImageEncoder Encoder { get; set; }

        public bool ReloadConfig { get; set; }
                        
        public static ConfigForm Create()
        {
            Builder builder = new Builder(null, "MakeACameraWithPiZero.MakeACameraWithPiZero.glade", null);
            return new ConfigForm(builder, builder.GetObject("ConfigWindow").Handle);
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="handle">  The handle. </param>
        protected ConfigForm(Builder builder, IntPtr handle) : base(handle)
        {            
            _builder = builder;
            builder.Autoconnect(this);

            //Create our component pipeline.        
            this.Handler = new ImageStreamCaptureHandler("/home/pi/images/makeacamera", "jpg");
            this.Encoder = new MMALImageEncoder(this.Handler);
                    
            this.Camera.AddEncoder(this.Encoder, this.Camera.Camera.StillPort)
               .CreatePreviewComponent(new MMALNullSinkComponent())
               .ConfigureCamera();

            ConfigureButton();
            InitialiseComboBoxes();
            SetupHandlers();
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
                      MMALCameraConfig.Reload();

                      AsyncContext.Run(async () =>
                      {
                          await this.Camera.TakePicture(this.Camera.Camera.StillPort, this.Camera.Camera.StillPort);
                      });                      
                  });

            this.ButtonConnection = new GpioConnection(switchButton);            
        }

        private void InitialiseComboBoxes()
        {
            //Sharpness ComboBox
            var sharpnessModel = new ListStore(typeof(int),
                                               typeof(string));

            this.SharpnessCombo.Model = sharpnessModel;

            for(int i = 0; i <= 10; i++)
            {
                sharpnessModel.AppendValues(i * 10, (i * 10).ToString());
            }

            //Set the default value for this combo box
            this.SharpnessCombo.Active = 4;
                        
            //Contrast ComboBox
            var contrastModel = new ListStore(typeof(int),
                                              typeof(string));

            this.ContrastCombo.Model = contrastModel;

            for (int i = 0; i <= 10; i++)
            {
                contrastModel.AppendValues(i * 10, (i * 10).ToString());
            }
                        
            this.ContrastCombo.Active = 4;

            //Brightness ComboBox
            var brightnessModel = new ListStore(typeof(int),
                                                typeof(string));

            this.BrightnessCombo.Model = brightnessModel;

            for (int i = 0; i <= 10; i++)
            {
                brightnessModel.AppendValues(i * 10, (i * 10).ToString());
            }

            this.BrightnessCombo.Active = 4;

            //Saturation ComboBox
            var saturationModel = new ListStore(typeof(int),
                                                typeof(string));

            this.SaturationCombo.Model = saturationModel;

            for (int i = 0; i <= 10; i++)
            {
                saturationModel.AppendValues(i * 10, (i * 10).ToString());
            }

            this.SaturationCombo.Active = 4;

            //ISO ComboBox
            var isoModel = new ListStore(typeof(int),
                                         typeof(string));

            this.ISOCombo.Model = isoModel;

            isoModel.AppendValues(100, "100");
            isoModel.AppendValues(200, "200");
            isoModel.AppendValues(400, "400");
            isoModel.AppendValues(800, "800");

            this.ISOCombo.Active = 0;
                        
            //Effects ComboBox
            var effectsModel = new ListStore(typeof(int),
                                             typeof(string));

            this.EffectsCombo.Model = effectsModel;

            foreach(MMAL_PARAM_IMAGEFX_T effect in Enum.GetValues(typeof(MMAL_PARAM_IMAGEFX_T)))
            {
                effectsModel.AppendValues(effect, effect.ToString());
            }

            //Image size ComboBox
            var imageSizeModel = new ListStore(typeof(string),
                                               typeof(string));

            this.ImageSizeCombo.Model = imageSizeModel;

            imageSizeModel.AppendValues("3264,2448", "8 Megapixel");
            imageSizeModel.AppendValues("3072,2304", "7 Megapixel");
            imageSizeModel.AppendValues("3032,2008", "6 Megapixel");
            imageSizeModel.AppendValues("2560,1920", "5 Megapixel");
            imageSizeModel.AppendValues("2240,1680", "4 Megapixel");
            imageSizeModel.AppendValues("2048,1536", "3 Megapixel");
            imageSizeModel.AppendValues("1600,1200", "2 Megapixel");
            imageSizeModel.AppendValues("1280,960", "1 Megapixel");
            imageSizeModel.AppendValues("640,480", "0.3 Megapixel");

            this.ImageSizeCombo.Active = 0;

        }

        /// <summary> Sets up the handlers. </summary>
        private void SetupHandlers()
        {
            this.DeleteEvent += new DeleteEventHandler(this.OnDestroy);
            this.SharpnessCombo.Changed += new EventHandler(this.OnSharpnessChanged);
            this.ContrastCombo.Changed += new EventHandler(this.OnContrastChanged);
            this.BrightnessCombo.Changed += new EventHandler(this.OnBrightnessChanged);
            this.SaturationCombo.Changed += new EventHandler(this.OnSaturationChanged);
            this.ISOCombo.Changed += new EventHandler(this.OnISOChanged);            
            this.EffectsCombo.Changed += new EventHandler(this.OnEffectChanged);
            this.ImageSizeCombo.Changed += new EventHandler(this.OnImageSizeChanged);
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

        private void OnDestroy(object o, DeleteEventArgs args)
        {
            Console.WriteLine("OnDestroy");
            this.Encoder.Dispose();
            this.Camera.Cleanup();
            this.ButtonConnection.Close();
            Application.Quit();
        }

    }
}
