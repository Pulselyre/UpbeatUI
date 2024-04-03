/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UpbeatUI.View
{
    internal static class ExtensionMethods
    {
        internal static UIElement GetRootChild(this ContentPresenter contentPresenter)
        {
            if (contentPresenter is null)
            {
                return null;
            }
            // When ContentPresenters do not have children, the content may not have been loaded yet.
            // Calling ".Measure" forces the template to be instantiated into content.
            if (VisualTreeHelper.GetChildrenCount(contentPresenter) == 0)
            {
                contentPresenter.Measure(new Size(0, 0));
            }
            return VisualTreeHelper.GetChildrenCount(contentPresenter) > 0
                ? VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement
                : null;
        }

        internal static void PercentMeasure(
            this UIElement element,
            Size constraint,
            string widthPercent,
            string heightPercent)
        {
            var child = element is ContentPresenter contentPresenter ? contentPresenter.GetRootChild() : element;
            (var _, var maxWidth) = (PercentPlace.GetWidthPercent(child) ?? widthPercent).ParsePercent(constraint.Width);
            (var _, var maxHeight) = (PercentPlace.GetHeightPercent(child) ?? heightPercent).ParsePercent(constraint.Height);
            element?.Measure(new Size(maxWidth, maxHeight));
        }

        internal static void PercentArrange(
            this UIElement element,
            Size arrangeBounds,
            string widthPercent,
            string heightPercent,
            string xPositionPercent,
            string yPositionPercent,
            bool keepInBounds)
        {
            var child = element is ContentPresenter contentPresenter ? contentPresenter.GetRootChild() : element;
            (var minWidth, var maxWidth) = (PercentPlace.GetWidthPercent(child) ?? widthPercent).ParsePercent(arrangeBounds.Width);
            (var minHeight, var maxHeight) = (PercentPlace.GetHeightPercent(child) ?? heightPercent).ParsePercent(arrangeBounds.Height);
            var width = Math.Min(maxWidth, Math.Max(minWidth, child.DesiredSize.Width));
            var height = Math.Min(maxHeight, Math.Max(minHeight, child.DesiredSize.Height));
            keepInBounds = PercentPlace.GetKeepInBounds(child) ?? keepInBounds;
            var xPosition = (PercentPlace.GetXPositionPercent(child) ?? xPositionPercent).ParsePercent();
            var yPosition = (PercentPlace.GetYPositionPercent(child) ?? yPositionPercent).ParsePercent();
            if (keepInBounds)
            {
                xPosition = Math.Max(
                    0, Math.Min(xPosition * arrangeBounds.Width - (width / 2.0), arrangeBounds.Width - width));
                yPosition = Math.Max(
                    0, Math.Min(yPosition * arrangeBounds.Height - (height / 2.0), arrangeBounds.Height - height));
            }
            else
            {
                xPosition = xPosition * arrangeBounds.Width - (width / 2.0);
                yPosition = yPosition * arrangeBounds.Height - (height / 2.0);
            }
            element?.Arrange(new Rect(xPosition, yPosition, width, height));
        }

        private static (double min, double max) ParsePercent(this string percentString, double available)
        {
            var sizes = percentString?.Split(' ').Select(s => ParsePercent(s) * available).ToList();
            return sizes == null || sizes.Count == 0
                ? (0.0, available)
                : (sizes[0], sizes.Count == 1 ? sizes[0] : sizes[1]
            );
        }

        private static double ParsePercent(this string percentString) =>
            percentString is null ? 0.5 : percentString.EndsWith('%')
                ? double.Parse(percentString.TrimEnd('%'), CultureInfo.InvariantCulture) / 100
                : double.Parse(percentString, CultureInfo.InvariantCulture);
    }
}
