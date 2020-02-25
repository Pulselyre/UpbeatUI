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
            var invert = values.Length < 4 ? false : (values[3] as bool?).GetValueOrDefault();
            var percentPosition = values[0] is string ? double.Parse(values[0] as string) : (values[0] as double?).GetValueOrDefault();
            var containerSize = (values[1] as double?).GetValueOrDefault();
            var controlSize = (values[2] as double?).GetValueOrDefault();
            if (parameter == null || !bool.Parse(parameter as string))
                return (invert ? 1.0 - percentPosition : percentPosition) * containerSize - (controlSize / 2.0);
            return Math.Max(
                0,
                Math.Min(
                    (invert ? 1.0 - percentPosition : percentPosition) * containerSize - (controlSize / 2.0),
                    containerSize - controlSize));
        }
    }
}
