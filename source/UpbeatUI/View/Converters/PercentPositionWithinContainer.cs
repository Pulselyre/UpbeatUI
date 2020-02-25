/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows;

namespace UpbeatUI.View.Converters
{
    public class PercentPositionWithinContainer : MultiValueConverterMarkupExtension<PercentPositionWithinContainer>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var container = values[0] as FrameworkElement;
            var control = values[1] as FrameworkElement;
            return new Func<Point>(() =>
            {
                var rawPoint = control.TranslatePoint(new Point(0, 0), container);
                return new Point(
                    (rawPoint.X + control.ActualWidth / 2.0) / container.ActualWidth,
                    (rawPoint.Y + control.ActualHeight / 2.0) / container.ActualHeight);
            });
        }
    }
}
