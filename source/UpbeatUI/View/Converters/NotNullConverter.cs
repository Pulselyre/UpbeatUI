/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Globalization;

namespace UpbeatUI.View.Converters
{
    public class NotNullConverter : ValueConverterMarkupExtension<NotNullConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value != null;
    }
}
