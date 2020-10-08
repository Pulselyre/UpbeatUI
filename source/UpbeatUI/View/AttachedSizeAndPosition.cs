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

        public static readonly DependencyProperty PositionViewModelroperty =
            DependencyProperty.RegisterAttached(
                "PositionViewModel",
                typeof(PositionViewModel),
                typeof(AttachedSizeAndPosition),
                new PropertyMetadata(
                    null,
                    PositionViewModelChangedHandler));

        public static readonly DependencyProperty SizeViewModelProperty =
            DependencyProperty.RegisterAttached(
                "SizeViewModel",
                typeof(SizeViewModel),
                typeof(AttachedSizeAndPosition),
                new PropertyMetadata(
                    null,
                    SizeViewModelChangedHandler));

        public static FrameworkElement GetContainer(FrameworkElement frameworkElement) =>
            (FrameworkElement)frameworkElement.GetValue(ContainerProperty);

        public static void SetContainer(FrameworkElement frameworkElement, FrameworkElement container) =>
            frameworkElement.SetValue(ContainerProperty, container);

        public static PositionViewModel GetPositionViewModel(FrameworkElement frameworkElement) =>
            (PositionViewModel)frameworkElement.GetValue(PositionViewModelroperty);

        public static void SetPositionViewModel(FrameworkElement frameworkElement, PositionViewModel positionViewModel) =>
            frameworkElement.SetValue(PositionViewModelroperty, positionViewModel);

        public static SizeViewModel GetSizeViewModel(FrameworkElement frameworkElement) =>
            (SizeViewModel)frameworkElement.GetValue(PositionViewModelroperty);

        public static void SetSizeViewModel(FrameworkElement frameworkElement, SizeViewModel sizeViewModel) =>
            frameworkElement.SetValue(PositionViewModelroperty, sizeViewModel);

        private static void PositionViewModelChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as FrameworkElement;
            if (target == null)
                return;
            var positionViewModel = e.NewValue as PositionViewModel;
            if (positionViewModel != null)
                positionViewModel.Finder = new Func<Point>(
                    () =>
                    {
                        var point = target.TranslatePoint(new Point(), GetContainer(target));
                        point.X += (target.FlowDirection == FlowDirection.LeftToRight ? 1 : -1) * (target.ActualWidth / 2.0);
                        point.Y += (target.ActualHeight / 2.0);
                        return point;
                    });
        }

        private static void SizeViewModelChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as FrameworkElement;
            if (target == null)
                return;
            var sizeViewModel = e.NewValue as SizeViewModel;
            if (sizeViewModel != null)
                sizeViewModel.Finder = new Func<Size>(() => new Size(target.ActualWidth, target.ActualHeight));
        }
    }
}
