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
                //case 1: return Convert.ToBoolean(value);
                case 2:
                    _control = GetNumericUpDown(0, byte.MinValue, byte.MaxValue);
                    break;
                case 3:
                    _control = GetNumericUpDown(0, int.MinValue, int.MaxValue);
                    break;
                case 4:
                    _control = GetNumericUpDown(0, long.MinValue, long.MaxValue);
                    break;
                case 5:
                    _control = GetNumericUpDown(6, long.MinValue, long.MaxValue);
                    break;
                case 6:
                    _control = GetNumericUpDown(6, long.MinValue, long.MaxValue);
                    break;
                case 7:
                    _control = GetNumericUpDown(6, long.MinValue, long.MaxValue);
                    break;
                //case 8: return Convert.ToDateTime(value);
                default:
                    _control = new TextBox() { Text = $"{prop.Value}" };
                    break;
            }
            if (n >= 3 && n <= 7)
                ((NumericUpDown)_control).Value = Convert.ToDecimal(prop.Value);

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
            return ((TextBox)_control).Text;
        }
    }
}
