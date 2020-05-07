using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NodeManager.Helpers
{
    internal static class TypeHelper
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ObservableCollection<T>(enumerable);
        }

        public static double ToDouble(this long num)
        {
            return Convert.ToDouble(num);
        }

        public static bool ToBool(this object obj)
        {
            return Convert.ToBoolean(obj);
        }
    }
}