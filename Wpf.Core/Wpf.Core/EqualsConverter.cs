using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Wpf.Core
{
    /// <summary>Converter that can be used to calculate if things are equal.</summary>
    /// <seealso cref="IValueConverter"/>
    public class EqualsConverter : IValueConverter
    {
        /// <summary>Determines if the bound value and the parameter are equal.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (targetType == typeof(string))
                {
                    string val = value?.ToString() ?? string.Empty;
                    string par = parameter?.ToString() ?? string.Empty;

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(int))
                {
                    int val = System.Convert.ToInt32(value);
                    int par = System.Convert.ToInt32(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(long))
                {
                    long val = System.Convert.ToInt64(value);
                    long par = System.Convert.ToInt64(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(float))
                {
                    float val = System.Convert.ToSingle(value);
                    float par = System.Convert.ToSingle(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(double))
                {
                    double val = System.Convert.ToDouble(value);
                    double par = System.Convert.ToDouble(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(bool))
                {
                    bool val = System.Convert.ToBoolean(value);
                    bool par = System.Convert.ToBoolean(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(byte))
                {
                    byte val = System.Convert.ToByte(value);
                    byte par = System.Convert.ToByte(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(short))
                {
                    short val = System.Convert.ToInt16(value);
                    short par = System.Convert.ToInt16(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(uint))
                {
                    uint val = System.Convert.ToUInt32(value);
                    uint par = System.Convert.ToUInt32(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(ulong))
                {
                    ulong val = System.Convert.ToUInt64(value);
                    ulong par = System.Convert.ToUInt64(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else if (targetType == typeof(ushort))
                {
                    ushort val = System.Convert.ToUInt16(value);
                    ushort par = System.Convert.ToUInt16(parameter);

                    if (val == par) return true;
                    else return false;
                }
                else
                {
                    if (value == null && parameter == null) return true;
                    else if (value == null && parameter != null) return false;
                    else if (value != null && parameter == null) return false;
                    else return value.Equals(parameter);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred during EqualsConverter.Convert. Object:{value} with TargetType:{targetType} that has Parameter:{parameter} using Culture:{culture}.{Environment.NewLine}{ex}");
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>Determines if the bound value and the parameter are equal.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
