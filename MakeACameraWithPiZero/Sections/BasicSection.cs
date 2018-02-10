using Gtk;
using MMALSharp;
using System;

namespace SwitchCam
{
    [Section(ContentType = typeof(ComboBox), Category = Category.Basic, Description = "Settings")]
    public class BasicSection : ListSection
    {
        public BasicSection()
        {
            AddItem(CreateBrightness());
            AddItem(CreateContrast());
            AddItem(CreateISO());
            AddItem(CreateShutterSpeed());
            AddItem(CreateSaturation());
            AddItem(CreateSharpness());
            AddItem(CreateSize());
        }

        public Tuple<string, Widget> CreateSharpness()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.Sharpness = int.Parse(value);
                ConfigForm.ReloadConfig = true;
            };

            // Sharpness ComboBox
            var sharpnessModel = new ListStore(typeof(int),
                                               typeof(string));

            dropdown.Model = sharpnessModel;

            for (int i = 0; i <= 10; i++)
            {
                sharpnessModel.AppendValues(i * 10, (i * 10).ToString());
            }

            // Set the default value for this combo box
            dropdown.Active = 4;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("Sharpness", dropdown);
        }

        public Tuple<string, Widget> CreateBrightness()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.Brightness = int.Parse(value);
                ConfigForm.ReloadConfig = true;
            };

            // Contrast ComboBox
            var contrastModel = new ListStore(typeof(int),
                                              typeof(string));

            dropdown.Model = contrastModel;

            for (int i = 0; i <= 10; i++)
            {
                contrastModel.AppendValues(i * 10, (i * 10).ToString());
            }

            dropdown.Active = 4;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("Brightness", dropdown);
        }

        public Tuple<string, Widget> CreateSaturation()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.Saturation = int.Parse(value);
                ConfigForm.ReloadConfig = true;
            };

            // Brightness ComboBox
            var brightnessModel = new ListStore(typeof(int),
                                                typeof(string));

            dropdown.Model = brightnessModel;

            for (int i = 0; i <= 10; i++)
            {
                brightnessModel.AppendValues(i * 10, (i * 10).ToString());
            }

            dropdown.Active = 4;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("Saturation", dropdown);
        }

        public Tuple<string, Widget> CreateContrast()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.Contrast = int.Parse(value);
                ConfigForm.ReloadConfig = true;
            };

            // Saturation ComboBox
            var saturationModel = new ListStore(typeof(int),
                                                typeof(string));

            dropdown.Model = saturationModel;

            for (int i = 0; i <= 10; i++)
            {
                saturationModel.AppendValues(i * 10, (i * 10).ToString());
            }

            dropdown.Active = 4;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("Contrast", dropdown);
        }

        public Tuple<string, Widget> CreateISO()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.ISO = int.Parse(value);
                ConfigForm.ReloadConfig = true;
            };

            // ISO ComboBox
            var isoModel = new ListStore(typeof(int),
                                         typeof(string));

            dropdown.Model = isoModel;

            isoModel.AppendValues(100, "100");
            isoModel.AppendValues(200, "200");
            isoModel.AppendValues(400, "400");
            isoModel.AppendValues(800, "800");

            dropdown.Active = 0;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("ISO", dropdown);
        }

        public Tuple<string, Widget> CreateShutterSpeed()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.ShutterSpeed = int.Parse(value);
                ConfigForm.ReloadConfig = true;
            };

            // ISO ComboBox
            var isoModel = new ListStore(typeof(int),
                                         typeof(string));

            dropdown.Model = isoModel;

            isoModel.AppendValues(1000000, "1000000");
            isoModel.AppendValues(2000000, "2000000");
            isoModel.AppendValues(3000000, "3000000");
            isoModel.AppendValues(4000000, "4000000");
            isoModel.AppendValues(5000000, "5000000");
            isoModel.AppendValues(6000000, "6000000");

            dropdown.Active = 0;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("Shutter speed", dropdown);
        }

        public Tuple<string, Widget> CreateSize()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {                
                var value = this.GetDropdownValue(dropdown);
                var split = value.Split('x');

                MMALCameraConfig.StillResolution = new Resolution(int.Parse(split[0].Trim()), int.Parse(split[1].Trim()));

                ConfigForm.ReloadConfig = true;
            };

            // Image size ComboBox
            var imageSizeModel = new ListStore(typeof(int), typeof(string));

            dropdown.Model = imageSizeModel;

            imageSizeModel.AppendValues(0, "3264 x 2448");
            imageSizeModel.AppendValues(1, "3072 x 2304");
            imageSizeModel.AppendValues(2, "3032 x 2008");
            imageSizeModel.AppendValues(3, "2560 x 1920");
            imageSizeModel.AppendValues(4, "2240 x 1680");
            imageSizeModel.AppendValues(5, "2048 x 1536");
            imageSizeModel.AppendValues(6, "1600 x 1200");
            imageSizeModel.AppendValues(7, "1280 x 960");
            imageSizeModel.AppendValues(8, "640 x 480");

            dropdown.Active = 0;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 1);

            return new Tuple<string, Widget>("Resolution", dropdown);
        }                
    }
}
