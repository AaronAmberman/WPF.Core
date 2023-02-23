using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Wpf.Core
{
    /// <summary>Converter that can be used to calculate if things are not equal.</summary>
    /// <seealso cref="IValueConverter"/>
    public class NotEqualsMultiConverter : IMultiValueConverter
    {
        /// <summary>Determines if the bound value and the parameter are equal.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool[] valuesCollection = new bool[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    object value = values[i];

                    if (targetType == typeof(string))
                    {
                        string val = value?.ToString() ?? string.Empty;
                        string par = parameter?.ToString() ?? string.Empty;

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(int))
                    {
                        int val = System.Convert.ToInt32(value);
                        int par = System.Convert.ToInt32(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(long))
                    {
                        long val = System.Convert.ToInt64(value);
                        long par = System.Convert.ToInt64(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(float))
                    {
                        float val = System.Convert.ToSingle(value);
                        float par = System.Convert.ToSingle(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(double))
                    {
                        double val = System.Convert.ToDouble(value);
                        double par = System.Convert.ToDouble(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(bool))
                    {
                        bool val = System.Convert.ToBoolean(value);
                        bool par = System.Convert.ToBoolean(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(byte))
                    {
                        byte val = System.Convert.ToByte(value);
                        byte par = System.Convert.ToByte(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(short))
                    {
                        short val = System.Convert.ToInt16(value);
                        short par = System.Convert.ToInt16(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(uint))
                    {
                        uint val = System.Convert.ToUInt32(value);
                        uint par = System.Convert.ToUInt32(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(ulong))
                    {
                        ulong val = System.Convert.ToUInt64(value);
                        ulong par = System.Convert.ToUInt64(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else if (targetType == typeof(ushort))
                    {
                        ushort val = System.Convert.ToUInt16(value);
                        ushort par = System.Convert.ToUInt16(parameter);

                        if (val == par) valuesCollection[i] = false;
                        else valuesCollection[i] = true;
                    }
                    else
                    {
                        if (value == null && parameter == null) valuesCollection[i] = false;
                        else if (value == null && parameter != null) valuesCollection[i] = true;
                        else if (value != null && parameter == null) valuesCollection[i] = true;
                        else valuesCollection[i] = !value.Equals(parameter);
                    }
                }

                return valuesCollection.All(x => !x); // all value must be false
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred during EqualsConverter.Convert. Objects:{values} with TargetType:{targetType} that has Parameter:{parameter} using Culture:{culture}.{Environment.NewLine}{ex}");
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>Determines if the bound value and the parameter are equal.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
