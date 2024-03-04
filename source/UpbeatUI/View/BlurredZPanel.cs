/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace UpbeatUI.View
{
    /// <summary>
    /// Defines an area where child elements are stacked vertically on top of each other.
    /// </summary>
    public class BlurredZPanel : Panel
    {
        /// <summary>
        /// Identifies the UpbeatUI.View.BlurredZPanel.BlurColor dependency property.
        /// </summary>
        public static readonly DependencyProperty BlurColorProperty
            = DependencyProperty.Register(
                "BlurColor",
                typeof(Brush),
                typeof(BlurredZPanel),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(Brushes.Gray.Color)
                    {
                        Opacity = 0.5,
                    },
                    FrameworkPropertyMetadataOptions.AffectsArrange));
        /// <summary>
        /// Identifies the UpbeatUI.View.BlurredZPanel.BlurRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty BlurRadiusProperty
            = DependencyProperty.Register(
                "BlurRadius",
                typeof(double),
                typeof(BlurredZPanel),
                new FrameworkPropertyMetadata(
                    10.0,
                    FrameworkPropertyMetadataOptions.AffectsArrange));
        /// <summary>
        /// Identifies the UpbeatUI.View.BlurredZPanel.ClosePopupCommand dependency property.
        /// </summary>
        public static readonly DependencyProperty ClosePopupCommandProperty
            = DependencyProperty.Register(
                "ClosePopupCommand",
                typeof(ICommand),
                typeof(BlurredZPanel));
        /// <summary>
        /// Identifies the UpbeatUI.View.BlurredZPanel.RequestPopupCloseEvent RoutedEvent.
        /// </summary>
        public static readonly RoutedEvent RequestPopupCloseEvent
            = EventManager.RegisterRoutedEvent(
                "RequestPopupClose",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(BlurredZPanel));

        private Border _rectangle;
        private Button _button;
        private bool _blockingVisualChange;

        /// <summary>
        /// Occurs after the user requested that the top Control be removed from ItemsSource.
        /// </summary>
        public event RoutedEventHandler PopupCloseRequested
        {
            add => AddHandler(RequestPopupCloseEvent, value);
            remove => RemoveHandler(RequestPopupCloseEvent, value);
        }

        /// <summary>
        /// Gets or sets a brush that the UpbeatStackControl will show underneath the top (active) Control.
        /// </summary>
        public Brush BlurColor
        {
            get => (Brush)GetValue(BlurColorProperty);
            set => SetValue(BlurColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the amount of blurring effect to show underneath the top (active) control.
        /// </summary>
        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        /// <summary>
        /// Gets or sets the ICommand to execute when the user requests that the top Control be removed from the ItemsSource.
        /// </summary>
        public ICommand ClosePopupCommand
        {
            get => (ICommand)GetValue(ClosePopupCommandProperty);
            set => SetValue(ClosePopupCommandProperty, value);
        }
        protected override int VisualChildrenCount =>
            base.VisualChildrenCount + (_rectangle == null ? 0 : 1) + (_button == null ? 0 : 1);

        protected override Size ArrangeOverride(Size finalSize)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                if (_rectangle != null && i == Children.Count - 1)
                {
                    _rectangle.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                    _button.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
                Children[i].PercentArrange(finalSize, null, null, null, null, false);
                if (i < Children.Count - 1)
                {
                    FocusManager.SetIsFocusScope(Children[i], false);
                    KeyboardNavigation.SetTabNavigation(Children[i], KeyboardNavigationMode.None);
                }
                else
                {
                    FocusManager.SetIsFocusScope(Children[i], true);
                    FocusManager.SetFocusedElement(Children[i], Children[i]);
                    KeyboardNavigation.SetTabNavigation(Children[i], KeyboardNavigationMode.Local);
                }
            }
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
            => VisualChildrenCount == 1 || index < VisualChildrenCount - 3 ? base.GetVisualChild(index)
                : index == VisualChildrenCount - 3 ? _rectangle
                : index == VisualChildrenCount - 2 ? _button
                : base.GetVisualChild(index - 2);

        protected override Size MeasureOverride(Size availableSize)
        {
            var desiredSize = new Size(0, 0);
            foreach (var child in Children.OfType<UIElement>())
            {
                child.PercentMeasure(availableSize, null, null);
                desiredSize.Width = Math.Max(desiredSize.Width, child.DesiredSize.Width);
                desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
            }
            if (Children.Count > 1)
            {
                _rectangle.Measure(availableSize);
                _button.Measure(availableSize);
            }
            return new Size(
                double.IsInfinity(availableSize.Width) ? desiredSize.Width : availableSize.Width,
                double.IsInfinity(availableSize.Height) ? desiredSize.Height : availableSize.Height);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (_blockingVisualChange)
            {
                return;
            }

            if (_rectangle != null)
            {
                _blockingVisualChange = true;
                RemoveVisualChild(_rectangle);
                RemoveVisualChild(_button);
                _blockingVisualChange = false;
                _rectangle = null;
                _button = null;
            }
            var blurEffect = new BlurEffect()
            {
                Radius = BlurRadius,
                KernelType = KernelType.Gaussian,
            };
            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i] != null)
                {
                    Children[i].Effect = blurEffect;
                }
            }

            for (var i = Children.Count - 1; i >= 0; i--)
            {
                if (Children[i] != null)
                {
                    Children[i].Effect = null;
                    i = -1;
                }
            }

            if ((visualAdded != null && Children.Count > 1)
                || (visualRemoved != null && Children.Count > 2))
            {
                _rectangle = new Border()
                {
                    Background = BlurColor,
                    BorderBrush = BlurColor,
                    Focusable = false,
                };
                _button = new Button()
                {
                    Opacity = 0.0,
                    Focusable = false,
                };
                _ = BindingOperations.SetBinding(
                    _button, ButtonBase.CommandProperty, new Binding()
                    {
                        Source = ClosePopupCommand,
                    });
                _blockingVisualChange = true;
                AddVisualChild(_rectangle);
                AddVisualChild(_button);
                _blockingVisualChange = false;
            }
        }

        private void CloseMouseUp(object sender, RoutedEventArgs e)
        {
            if (ClosePopupCommand?.CanExecute(null) ?? false)
            {
                ClosePopupCommand.Execute(null);
            }
            else
            {
                RaiseEvent(new RoutedEventArgs(RequestPopupCloseEvent));
            }
        }

        private void CloseTouchUp(object sender, TouchEventArgs e)
            => CloseMouseUp(sender, null);
    }
}
