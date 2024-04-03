/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows.Data;

namespace UpbeatUI.View.Converters
{
    /// <summary>
    /// A converter that accepts multiple values to convert a percentage of available dimensions to a position.
    /// </summary>
    [Obsolete("'" + nameof(PercentPositionPlacementConverter) + "' method is deprecated and will be removed in the next major release; consider using the '" + nameof(PercentPlaceContentControl) + "' class to position elements instead.")]
    public class PercentPositionPlacementConverter : IMultiValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));
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
            return keepInBounds
                ? Math.Max(
                    0,
                    Math.Min(
                        (invert ? 1.0 - percentPosition : percentPosition) * containerSize - (controlSize / 2.0),
                        containerSize - controlSize))
                : (object)((invert ? 1.0 - percentPosition : percentPosition) * containerSize - (controlSize / 2.0));
        }

        /// <inheritdoc/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
