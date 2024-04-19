/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UpbeatUI.View
{
    /// <summary>
    /// Defines an area where child elements are stacked vertically on top of each other.
    /// </summary>
    public class ModalPanel : Panel
    {
        /// <summary>
        /// Identifies the <see cref="ModalBackground"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty ModalBackgroundProperty =
            DependencyProperty.Register(
                "ModalBackground",
                typeof(Brush),
                typeof(ModalPanel),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(Brushes.Gray.Color) { Opacity = 0.5 },
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies the <see cref="ClosePopupCommand"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty ClosePopupCommandProperty =
            DependencyProperty.Register(
                "ClosePopupCommand",
                typeof(ICommand),
                typeof(ModalPanel));

        /// <summary>
        /// Identifies the IsOnTop Attached <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty IsOnTopProperty =
            DependencyProperty.RegisterAttached(
                "IsOnTop",
                typeof(bool?),
                typeof(ModalPanel),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="RequestPopupCloseEvent"/> <see cref="RoutedEvent"/>.
        /// </summary>
        public static readonly RoutedEvent RequestPopupCloseEvent =
            EventManager.RegisterRoutedEvent(
                "RequestPopupClose",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ModalPanel));

        private Border _border;
        private bool _isBorderDown;
        private bool _blockingVisualChange;

        /// <summary>
        /// Gets or sets a <see cref="Brush"/> that will be shown underneath the top (active) Element>.
        /// </summary>
        public Brush ModalBackground
        {
            get => (Brush)GetValue(ModalBackgroundProperty);
            set => SetValue(ModalBackgroundProperty, value);
        }

        /// <summary>
        /// Occurs after the user requested that the top Element be removed.
        /// </summary>
        public event RoutedEventHandler PopupCloseRequested
        {
            add => AddHandler(RequestPopupCloseEvent, value);
            remove => RemoveHandler(RequestPopupCloseEvent, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="ICommand"/> to execute when the user requests that the top Element be removed.
        /// </summary>
        public ICommand ClosePopupCommand
        {
            get => (ICommand)GetValue(ClosePopupCommandProperty);
            set => SetValue(ClosePopupCommandProperty, value);
        }

        /// <summary>
        /// Gets whether <paramref name="dependencyObject"/> is the top element in a <see cref="ModalPanel"/>.
        /// </summary>
        /// <param name="dependencyObject">The element to get the IsOnTop sttaus of.</param>
        /// <returns> true if <paramref name="dependencyObject "/> is the top element, false if not the top element, and null if not part of a <see cref="ModalPanel"/> at all.</returns>
        public static bool? GetIsOnTop(DependencyObject dependencyObject) =>
            dependencyObject?.GetValue(IsOnTopProperty) as bool?;

        private static void SetIsOnTop(DependencyObject frameworkElement, bool? value) =>
            frameworkElement?.SetValue(IsOnTopProperty, value);

        /// <inheritdoc/>
        protected override int VisualChildrenCount =>
            base.VisualChildrenCount + (_border == null ? 0 : 1);

        /// <inheritdoc/>
        protected override Visual GetVisualChild(int index)
        {
            var childrenCount = VisualChildrenCount;
            var ret = _border == null || index < childrenCount - 2
                ? base.GetVisualChild(index)
                : index == childrenCount - 2 ? _border
                : base.GetVisualChild(index - 1);
            return ret;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            var desiredSize = new Size(0, 0);
            foreach (var child in Children.OfType<UIElement>())
            {
                child.PercentMeasure(availableSize, null, null);
                desiredSize.Width = Math.Max(desiredSize.Width, child.DesiredSize.Width);
                desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
            }
            _border?.Measure(availableSize);
            return new Size(
                double.IsInfinity(availableSize.Width) ? desiredSize.Width : availableSize.Width,
                double.IsInfinity(availableSize.Height) ? desiredSize.Height : availableSize.Height);
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].PercentArrange(finalSize, null, null, null, null, false);
                if (i < Children.Count - 1)
                {
                    SetIsOnTop(Children[i] as FrameworkElement, false);
                    FocusManager.SetIsFocusScope(Children[i], false);
                    KeyboardNavigation.SetTabNavigation(Children[i], KeyboardNavigationMode.None);
                }
                else
                {
                    SetIsOnTop(Children[i] as FrameworkElement, true);
                    FocusManager.SetIsFocusScope(Children[i], true);
                    FocusManager.SetFocusedElement(Children[i], Children[i]);
                    KeyboardNavigation.SetTabNavigation(Children[i], KeyboardNavigationMode.Local);
                }
            }
            if (_border != null)
            {
                _border.Background = ModalBackground;
                _border.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            return finalSize;
        }

        /// <inheritdoc/>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (!_blockingVisualChange)
            {
                _blockingVisualChange = true;
                if (visualAdded != null && Children.Count > 1)
                {
                    if (_border == null)
                    {
                        _border = new Border()
                        {
                            Focusable = false,
                            IsHitTestVisible = true,
                        };
                        _border.MouseDown += HandleBorderMouseDown;
                        _border.MouseUp += HandleBorderMouseUp;
                        _border.TouchDown += HandleBorderTouchDown;
                        _border.TouchUp += HandleBorderTouchUp;
                    }
                    else
                    {
                        RemoveVisualChild(_border);
                    }
                    AddVisualChild(_border);
                }
                if (visualRemoved != null)
                {
                    SetIsOnTop(visualRemoved as FrameworkElement, null);
                    RemoveVisualChild(_border);
                    if (Children.Count > 2)
                    {
                        AddVisualChild(_border);
                    }
                    else if (_border != null)
                    {
                        _border.MouseDown -= HandleBorderMouseDown;
                        _border.MouseUp -= HandleBorderMouseUp;
                        _border.TouchDown -= HandleBorderTouchDown;
                        _border.TouchUp -= HandleBorderTouchUp;
                        _border = null;
                    }
                }
                _blockingVisualChange = false;
            }
        }

        private void HandleBorderMouseDown(object sender, MouseButtonEventArgs e) => _isBorderDown = true;

        private void HandleBorderTouchDown(object sender, TouchEventArgs e) => _isBorderDown = true;

        private void HandleBorderMouseUp(object sender, RoutedEventArgs e)
        {
            if (_isBorderDown)
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
            _isBorderDown = false;
        }

        private void HandleBorderTouchUp(object sender, TouchEventArgs e)
            => HandleBorderMouseUp(sender, null);
    }
}
