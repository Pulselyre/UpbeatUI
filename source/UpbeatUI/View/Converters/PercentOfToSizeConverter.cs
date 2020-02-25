/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Globalization;

namespace UpbeatUI.View.Converters
{
    public class PercentOfToSizeConverter : ValueConverterMarkupExtension<PercentOfToSizeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (double)value * double.Parse(parameter as string) * 0.01;
    }
}
