/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows.Data;

namespace UpbeatUI.View.Converters
{
    public class PercentPositionPlacementConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var percentPosition = values[0] switch
            {
                string s => s.ParsePercent(),
                double d => d,
                _ => 0.5,
            };
            var containerSize = (values[1] as double?).GetValueOrDefault();
            var controlSize = (values[2] as double?).GetValueOrDefault();
            var invert = values[3] switch { bool v => v, string v => bool.Parse(v), _ => false };
            var keepInBounds = values[4] switch { bool v => v, string v => bool.Parse(v), _ => false };
            if (keepInBounds)
                return Math.Max(
                    0,
                    Math.Min(
                        (invert ? 1.0 - percentPosition : percentPosition) * containerSize - (controlSize / 2.0),
                        containerSize - controlSize));
            return (invert ? 1.0 - percentPosition : percentPosition) * containerSize - (controlSize / 2.0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
