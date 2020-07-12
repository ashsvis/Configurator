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
        DateTimePicker _time;

        /// <summary>
        /// Конструктор, в которм создаётся требуемый редактор
        /// </summary>
        /// <param name="prop"></param>
        public ValueEditorForm(ModelProperty prop)
        {
            InitializeComponent();
            _prop = prop;
            _time = new DateTimePicker() { ShowUpDown = true, Format = DateTimePickerFormat.Time, TabIndex = 1 };
            var n = 0;
            foreach (var typ in Helper.GetPropTypes())
            {
                if (typ.Type == prop.Type) break;
                n++;
            }
            int width = 214;
            int height = 23;
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
                    DateTime value = prop.Value == null ? _time.MinDate : (DateTime)prop.Value;
                    width = 122;
                    _control = new DateTimePicker() { Value = value.Date, Location = new Point(12, 12), Size = new Size(width, height), TabIndex = 0 };
                    Controls.Add(_control);
                    _time.Location = new Point(_control.Left + _control.Width + 12, 12);
                    _time.Size = new Size(80, height);
                    _time.Value = value;
                    Controls.Add(_time);
                    break;
                default:
                    Text = "Enter a text value here";
                    _control = new TextBox() { Text = $"{prop.Value}" };
                    break;
            }
            // числовые типы
            if (n >= 2 && n <= 7)
            {
                ((NumericUpDown)_control).Value = Convert.ToDecimal(prop.Value);
                ((NumericUpDown)_control).Select(0, 10);
            }
            // только дата/время
            if (n != 8)
            {
                _control.Location = new Point(12, 12);
                _control.Size = new Size(width, height);
                _control.TabIndex = 0;
                Controls.Add(_control);
            }
        }

        /// <summary>
        /// Возвращается редактор числового значения
        /// </summary>
        /// <param name="decimalPlaces"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Возврат изменённого значения
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            if (_control is NumericUpDown)
                return ((NumericUpDown)_control).Value;
            if (_control is DateTimePicker)
            {
                var dt = ((DateTimePicker)_control).Value.Date;
                var tm = _time.Value.TimeOfDay;
                return dt + tm;
            }
            if (_control is CheckBox)
                return ((CheckBox)_control).Checked;
            return ((TextBox)_control).Text;
        }
    }
}
