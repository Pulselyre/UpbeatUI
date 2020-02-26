/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace UpbeatUI.View.Converters
{
    public class PercentPositionWithinContextStackConverter : ValueConverterMarkupExtension<PercentPositionWithinContextStackConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var control = value as FrameworkElement;
            return new Func<Point>(() =>
            {
                var container = (FrameworkElement)VisualTreeHelper.GetParent(control);
                while (container is UpbeatStackControl == false)
                    container = (FrameworkElement)VisualTreeHelper.GetParent(container);
                var rawPoint = control.TranslatePoint(new Point(0, 0), container);
                return new Point(
                    (rawPoint.X + control.ActualWidth / 2.0) / container.ActualWidth,
                    (rawPoint.Y + control.ActualHeight / 2.0) / container.ActualHeight);
            });
        }
    }
}
