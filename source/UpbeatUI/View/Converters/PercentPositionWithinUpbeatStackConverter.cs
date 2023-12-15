/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UpbeatUI.View.Converters
{
    public class PercentPositionWithinUpbeatStackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var control = value as FrameworkElement;
            return new Func<Point>(() =>
            {
                var container = VisualTreeHelper.GetParent(control);
                while (container is UpbeatStackControl == false)
                {
                    container = VisualTreeHelper.GetParent(container);
                }
                var upbeatStackControl = container as UpbeatStackControl;
                var rawPoint = control.TranslatePoint(new Point(0, 0), upbeatStackControl);
                return new Point(
                    (rawPoint.X + control.ActualWidth / 2.0) / upbeatStackControl.ActualWidth,
                    (rawPoint.Y + control.ActualHeight / 2.0) / upbeatStackControl.ActualHeight);
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
