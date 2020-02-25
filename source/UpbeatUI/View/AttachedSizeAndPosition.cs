/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;
using UpbeatUI.Context;

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

        public static readonly DependencyProperty PositionContextProperty =
            DependencyProperty.RegisterAttached(
                "PositionContext",
                typeof(PositionContext),
                typeof(AttachedSizeAndPosition),
                new PropertyMetadata(
                    null,
                    PositionContextChangedHandler));

        public static readonly DependencyProperty SizeContextProperty =
            DependencyProperty.RegisterAttached(
                "SizeContext",
                typeof(SizeContext),
                typeof(AttachedSizeAndPosition),
                new PropertyMetadata(
                    null,
                    SizeContextChangedHandler));

        public static FrameworkElement GetContainer(FrameworkElement frameworkElement) =>
            (FrameworkElement)frameworkElement.GetValue(ContainerProperty);

        public static void SetContainer(FrameworkElement frameworkElement, FrameworkElement container) =>
            frameworkElement.SetValue(ContainerProperty, container);

        public static PositionContext GetPositionContext(FrameworkElement frameworkElement) =>
            (PositionContext)frameworkElement.GetValue(PositionContextProperty);

        public static void SetPositionContext(FrameworkElement frameworkElement, PositionContext positionContext) =>
            frameworkElement.SetValue(PositionContextProperty, positionContext);

        public static SizeContext GetSizeContext(FrameworkElement frameworkElement) =>
            (SizeContext)frameworkElement.GetValue(PositionContextProperty);

        public static void SetSizeContext(FrameworkElement frameworkElement, SizeContext positionContext) =>
            frameworkElement.SetValue(PositionContextProperty, positionContext);

        private static void PositionContextChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as FrameworkElement;
            if (target == null)
                return;
            var positionContext = e.NewValue as PositionContext;
            if (positionContext != null)
                positionContext.Finder = new Func<Point>(
                    () =>
                    {
                        var point = target.TranslatePoint(new Point(), GetContainer(target));
                        point.X += (target.FlowDirection == FlowDirection.LeftToRight ? 1 : -1) * (target.ActualWidth / 2.0);
                        point.Y += (target.ActualHeight / 2.0);
                        return point;
                    });
        }

        private static void SizeContextChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as FrameworkElement;
            if (target == null)
                return;
            var sizeContext = e.NewValue as SizeContext;
            if (sizeContext != null)
                sizeContext.Finder = new Func<Size>(() => new Size(target.ActualWidth, target.ActualHeight));
        }
    }
}
