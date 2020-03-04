/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UpbeatUI.View.Converters
{
    public abstract class ValueConverterMarkupExtension<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        private static T _converter = new T();

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _converter;

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
