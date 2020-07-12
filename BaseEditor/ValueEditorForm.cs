using ModelHolder;
using ModelHolder.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BaseEditor
{
    public partial class ValueEditorForm : Form
    {
        ModelProperty _prop;
        Control _control = null;

        public ValueEditorForm(ModelProperty prop)
        {
            InitializeComponent();
            _prop = prop;
            var n = 0;
            foreach (var typ in Helper.GetPropTypes())
            {
                if (typ.Type == prop.Type) break;
                n++;
            }
            switch (n)
            {
                case 1:
                    Text = "Enter a bool value here";
                    _control = new CheckBox() { Checked = Convert.ToBoolean(prop.Value) };
                    _control.Text = $"{((CheckBox)_control).Checked}";
                    _control.Click += (o, e) => { ((CheckBox)o).Text = $"{((CheckBox)o).Checked}"; };
                    break;
                case 2:
                    Text = "Enter a byte value here";
                    _control = GetNumericUpDown(0, byte.MinValue, byte.MaxValue);
                    break;
                case 3:
                    Text = "Enter a integer value here";
                    _control = GetNumericUpDown(0, int.MinValue, int.MaxValue);
                    break;
                case 4:
                    Text = "Enter a long value here";
                    _control = GetNumericUpDown(0, long.MinValue, long.MaxValue);
                    break;
                case 5:
                    Text = "Enter a float value here";
                    _control = GetNumericUpDown(6, long.MinValue, long.MaxValue);
                    break;
                case 6:
                    Text = "Enter a double value here";
                    _control = GetNumericUpDown(6, long.MinValue, long.MaxValue);
                    break;
                case 7:
                    Text = "Enter a decimal value here";
                    _control = GetNumericUpDown(6, long.MinValue, long.MaxValue);
                    break;
                case 8:
                    Text = "Enter a date and time value here";
                    _control = new DateTimePicker() { Value = Convert.ToDateTime(prop.Value) };
                    break;
                default:
                    Text = "Enter a text value here";
                    _control = new TextBox() { Text = $"{prop.Value}" };
                    break;
            }
            if (n >= 2 && n <= 7)
            {
                ((NumericUpDown)_control).Value = Convert.ToDecimal(prop.Value);
                ((NumericUpDown)_control).Select(0, 10);
            }
            _control.Location = new Point(12, 12);
            _control.Size = new Size(214, 23);
            _control.TabIndex = 0;
            Controls.Add(_control);
        }

        private static Control GetNumericUpDown(int decimalPlaces, double minValue, double maxValue)
        {
            return new NumericUpDown()
            {
                TextAlign = HorizontalAlignment.Right,
                DecimalPlaces = decimalPlaces,
                Minimum = (decimal)minValue,
                Maximum = (decimal)maxValue
            };
        }

        public object GetValue()
        {
            if (_control is NumericUpDown)
                return ((NumericUpDown)_control).Value;
            if (_control is DateTimePicker)
                return ((DateTimePicker)_control).Value;
            if (_control is CheckBox)
                return ((CheckBox)_control).Checked;
            return ((TextBox)_control).Text;
        }
    }
}
