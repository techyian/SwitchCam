using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private ComboBox ShutterSpeedCombo;

        [Builder.Object]
        private ComboBox EffectsCombo;

        [Builder.Object]
        private ComboBox ImageSizeCombo;
        
        public static ConfigForm Create()
        {
            Builder builder = new Builder(null, "MakeACameraWithPiZero.glade", null);
            return new ConfigForm(builder, builder.GetObject("ConfigWindow").Handle);
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="handle">  The handle. </param>
        protected ConfigForm(Builder builder, IntPtr handle) : base(handle)
        {
            _builder = builder;
            builder.Autoconnect(this);
            InitialiseComboBoxes();
            SetupHandlers();
        }

        protected void InitialiseComboBoxes()
        {            
        }

        /// <summary> Sets up the handlers. </summary>
        protected void SetupHandlers()
        {            
        }


    }
}
