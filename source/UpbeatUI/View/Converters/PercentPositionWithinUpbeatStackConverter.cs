/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UpbeatUI.ViewModel;

namespace UpbeatUI.View.Converters
{
    /// <summary>
    /// A converter that creates a <see cref="Func{Point}"/> that returns the location within an <see cref="IUpbeatStack"/> of the target element.
    /// </summary>
    public class PercentPositionWithinUpbeatStackConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var containerClass = parameter is null ? typeof(IUpbeatStack) : (Type)parameter;
            var control = value as FrameworkElement;
            return new Func<Point>(() =>
            {
                var container = control as DependencyObject;
                var parent = VisualTreeHelper.GetParent(control);
                while (!(parent is null))
                {
                    if (parent is FrameworkElement parentElement &&
                        containerClass.IsAssignableFrom(parentElement.DataContext.GetType()))
                    {
                        var rawPoint = control.TranslatePoint(new Point(0, 0), parentElement);
                        return new Point(
                            (rawPoint.X + control.ActualWidth / 2.0) / parentElement.ActualWidth,
                            (rawPoint.Y + control.ActualHeight / 2.0) / parentElement.ActualHeight);
                    }
                    container = parent;
                    parent = VisualTreeHelper.GetParent(container);
                }
                throw new InvalidOperationException($"Unable to locate an ancestor {nameof(IUpbeatStack)} to position within. Reached root element {container}.");
            });
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
