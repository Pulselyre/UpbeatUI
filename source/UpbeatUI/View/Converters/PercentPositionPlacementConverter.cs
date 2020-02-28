/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Globalization;

namespace UpbeatUI.View.Converters
{
    public class PercentPositionPlacementConverter : MultiValueConverterMarkupExtension<PercentPositionPlacementConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var percentPosition = values[0] is string ? double.Parse(values[0] as string) : (values[0] as double?).GetValueOrDefault();
            var containerSize = (values[1] as double?).GetValueOrDefault();
            var controlSize = (values[2] as double?).GetValueOrDefault();
            var invert = (values[3] as bool?).GetValueOrDefault();
            var keepInBounds = (values[4] as bool?).GetValueOrDefault();
            if (keepInBounds)
                return Math.Max(
                    0,
                    Math.Min(
                        (invert ? 1.0 - percentPosition : percentPosition) * containerSize - (controlSize / 2.0),
                        containerSize - controlSize));
            return (invert ? 1.0 - percentPosition : percentPosition) * containerSize - (controlSize / 2.0);
        }
    }
}
