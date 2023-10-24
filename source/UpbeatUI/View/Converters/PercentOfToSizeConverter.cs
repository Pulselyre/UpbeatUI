/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows.Data;

namespace UpbeatUI.View.Converters
{
    public class PercentOfToSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sizes = (values[0] as string)?.Split(' ');
            if (sizes == null)
                return (parameter as string)?.ToLower() switch
                {
                    "min" => 0,
                    "size" => double.NaN,
                    "max" => double.PositiveInfinity,
                    _ => throw new ArgumentException("Invalid size string"),
                };
            var containerSize = (values[1] as double?).GetValueOrDefault();
            return (parameter as string)?.ToLower() switch
            {
                "min" => sizes.Length switch
                {
                    2 => sizes[0].ParsePercent() * containerSize,
                    _ => 0,
                },
                "size" => sizes.Length switch
                {
                    1 => sizes[0].ParsePercent() * containerSize,
                    _ => double.NaN,
                },
                "max" => sizes.Length switch
                {
                    2 => sizes[1].ParsePercent() * containerSize,
                    _ => double.PositiveInfinity,
                },
                _ => throw new ArgumentException("Invalid size string"),
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
