using Gtk;
using MMALSharp;
using MMALSharp.Native;
using SwitchCam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeACameraWithPiZero.Sections
{
    [Section(ContentType = typeof(ComboBox), Category = Category.Basic, Description = "Effects")]
    public class EffectsSection : ListSection
    {
        public EffectsSection()
        {
            AddItem(CreateSpecialEffects());
            AddItem(CreateAwbMode());
            AddItem(CreateExposureMode());
            AddItem(CreateExposureCompensation());
            AddItem(CreateRotation());
        }

        public Tuple<string, Widget> CreateSpecialEffects()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.ImageFx = (MMAL_PARAM_IMAGEFX_T)Enum.Parse(typeof(MMAL_PARAM_IMAGEFX_T), value);
                ConfigForm.ReloadConfig = true;
            };

            // Effects ComboBox
            var effectsModel = new ListStore(typeof(string),
                                             typeof(string));

            dropdown.Model = effectsModel;

            var enums = Enum.GetValues(typeof(MMAL_PARAM_IMAGEFX_T)).Cast<MMAL_PARAM_IMAGEFX_T>().ToList();
            for (var i = 0; i < enums.Count; i++)
            {
                var split = enums[i].ToString().Split('_');
                effectsModel.AppendValues(split.Last(), enums[i].ToString());
            }

            dropdown.Active = 0;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("ImageFx", dropdown);
        }

        public Tuple<string, Widget> CreateAwbMode()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.AwbMode = (MMAL_PARAM_AWBMODE_T)Enum.Parse(typeof(MMAL_PARAM_AWBMODE_T), value);
                ConfigForm.ReloadConfig = true;
            };
            
            var awbModel = new ListStore(typeof(string),
                                             typeof(string));

            dropdown.Model = awbModel;

            var enums = Enum.GetValues(typeof(MMAL_PARAM_AWBMODE_T)).Cast<MMAL_PARAM_AWBMODE_T>().ToList();
            for (var i = 0; i < enums.Count; i++)
            {
                var split = enums[i].ToString().Split('_');
                awbModel.AppendValues(split.Last(), enums[i].ToString());
            }

            dropdown.Active = 1;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("AWB Mode", dropdown);
        }

        public Tuple<string, Widget> CreateExposureMode()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.ExposureMode = (MMAL_PARAM_EXPOSUREMODE_T)Enum.Parse(typeof(MMAL_PARAM_EXPOSUREMODE_T), value);
                ConfigForm.ReloadConfig = true;
            };

            var expModeModel = new ListStore(typeof(string),
                                             typeof(string));

            dropdown.Model = expModeModel;

            var enums = Enum.GetValues(typeof(MMAL_PARAM_EXPOSUREMODE_T)).Cast<MMAL_PARAM_EXPOSUREMODE_T>().ToList();
            for (var i = 0; i < enums.Count; i++)
            {
                var split = enums[i].ToString().Split('_');
                expModeModel.AppendValues(split.Last(), enums[i].ToString());
            }

            dropdown.Active = 1;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("Exposure Mode", dropdown);
        }

        public Tuple<string, Widget> CreateExposureCompensation()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.ExposureCompensation = int.Parse(value);
                ConfigForm.ReloadConfig = true;
            };

            var expModel = new ListStore(typeof(int),
                                         typeof(string));

            dropdown.Model = expModel;

            for (int i = -10; i <= 10; i++)
            {
                expModel.AppendValues(i, i.ToString());
            }

            // Set the default value for this combo box
            dropdown.Active = 10;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("Exp Compensation", dropdown);
        }

        public Tuple<string, Widget> CreateRotation()
        {
            var dropdown = new ComboBox();
            dropdown.Changed += (sender, e) =>
            {
                var value = this.GetDropdownValue(dropdown);
                MMALCameraConfig.Rotation = int.Parse(value);
                ConfigForm.ReloadConfig = true;
            };

            var rotModel = new ListStore(typeof(int),
                                         typeof(string));

            dropdown.Model = rotModel;

            rotModel.AppendValues(0, "0");
            rotModel.AppendValues(90, "90");
            rotModel.AppendValues(180, "180");
            rotModel.AppendValues(270, "270");

            // Set the default value for this combo box
            dropdown.Active = 0;

            CellRendererText text = new CellRendererText();
            dropdown.PackStart(text, false);
            dropdown.AddAttribute(text, "text", 0);

            return new Tuple<string, Widget>("Rotation", dropdown);
        }
    }
}
