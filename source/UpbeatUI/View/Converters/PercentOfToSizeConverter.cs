/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows.Data;

namespace UpbeatUI.View.Converters
{
    public class PercentOfToSizeConverter : MultiValueConverterMarkupExtension<PercentOfToSizeConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sizes = (values[0] as string)?.Split(' ') ?? throw new ArgumentException("Binding [0] must be a string of percent size values");
            var containerSize = (values[1] as double?).GetValueOrDefault();
            if (sizes.Length == 1)
                return double.Parse(sizes[0]) * containerSize;
            if (sizes.Length == 3)
                return Math.Max(
                    double.Parse(sizes[0]) * containerSize,
                    Math.Min(
                        double.Parse(sizes[1]) * containerSize,
                        double.Parse(sizes[2]) * containerSize));
            throw new ArgumentException("Binding [0] must be a string of 1 (absolute) or 3 percent size values (min, preferred, max)");
        }
    }
}
