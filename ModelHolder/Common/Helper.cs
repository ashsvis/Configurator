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
                typeof(decimal)
            };
            return list.Select(x => new TypeItem { Type = x }) .ToArray();
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
