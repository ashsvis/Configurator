using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelHolder.Common
{
    public static class Helper
    {
        public static TypeItem[] GetPropTypes()
        {
            var list = new List<Type>
            {
                typeof(string),
                typeof(bool),
                typeof(byte),
                typeof(int),
                typeof(long),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(DateTime)
            };
            return list.Select(x => new TypeItem { Type = x }) .ToArray();
        }

        public static object ConvertFrom(Type type, object value)
        {
            var n = 0;
            foreach (var typ in GetPropTypes())
            {
                if (typ.Type == type) break;
                n++;
            }
            switch (n)
            {
                case 0: return Convert.ToString(value);
                case 1: return Convert.ToBoolean(value);
                case 2: return Convert.ToByte(value);
                case 3: return Convert.ToInt32(value);
                case 4: return Convert.ToInt64(value);
                case 5: return Convert.ToSingle(value);
                case 6: return Convert.ToDouble(value);
                case 7: return Convert.ToDecimal(value);
                case 8: return Convert.ToDateTime(value);
                default:
                    return value;
            }
        }
    }

    public class TypeItem
    {
        public Type Type { get; set; }

        public override string ToString()
        {
            var vals = Type.ToString().Split('.');
            return vals[vals.Length - 1];
        }
    }
}
