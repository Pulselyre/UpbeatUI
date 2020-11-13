/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;
using UpbeatUI.ViewModel;

namespace UpbeatUI.View
{
    public class AttachedSizeAndPosition
    {
        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.RegisterAttached(
                "Container",
                typeof(FrameworkElement),
                typeof(AttachedSizeAndPosition),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PositionRetrieverProperty =
            DependencyProperty.RegisterAttached(
                "PositionRetriever",
                typeof(PositionRetriever),
                typeof(AttachedSizeAndPosition),
                new PropertyMetadata(null, PositionRetrieverChangedHandler));

        public static readonly DependencyProperty SizeRetrieverProperty =
            DependencyProperty.RegisterAttached(
                "SizeRetriever",
                typeof(SizeRetriever),
                typeof(AttachedSizeAndPosition),
                new PropertyMetadata(null, SizeRetrieverChangedHandler));

        public static FrameworkElement GetContainer(FrameworkElement frameworkElement) =>
            (FrameworkElement)frameworkElement.GetValue(ContainerProperty);

        public static void SetContainer(FrameworkElement frameworkElement, FrameworkElement container) =>
            frameworkElement.SetValue(ContainerProperty, container);

        public static PositionRetriever GetPositionRetriever(FrameworkElement frameworkElement) =>
            (PositionRetriever)frameworkElement.GetValue(PositionRetrieverProperty);

        public static void SetPositionRetriever(FrameworkElement frameworkElement, PositionRetriever positionRetriever) =>
            frameworkElement.SetValue(PositionRetrieverProperty, positionRetriever);

        public static SizeRetriever GetSizeRetriever(FrameworkElement frameworkElement) =>
            (SizeRetriever)frameworkElement.GetValue(SizeRetrieverProperty);

        public static void SetSizeRetriever(FrameworkElement frameworkElement, SizeRetriever sizeRetriever) =>
            frameworkElement.SetValue(SizeRetrieverProperty, sizeRetriever);

        private static void PositionRetrieverChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement target))
                return;
            GetPositionRetriever(target).Retriever = () =>
            {
                var container = GetContainer(target) ?? throw new InvalidOperationException($"The {nameof(AttachedSizeAndPosition.ContainerProperty)} has not been initialized.");
                var point = target.TranslatePoint(new Point(), container);
                point.X += (target.FlowDirection == FlowDirection.LeftToRight ? 1 : -1) * (target.ActualWidth / 2.0);
                point.Y += (target.ActualHeight / 2.0);
                return point;
            };
        }

        private static void SizeRetrieverChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement target))
                return;
            GetSizeRetriever(target).Retriever = () => new Size(target.ActualWidth, target.ActualHeight);
        }
    }
}
