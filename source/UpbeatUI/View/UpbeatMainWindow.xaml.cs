/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace UpbeatUI.View
{
    /// <summary>
    /// Defines a window with a pre-embedded <see cref="ModalPanel"/> to stack Views. The <see cref="FrameworkElement.DataContext"/> property should be set to an <see cref="ViewModel.IUpbeatStack"/> instance.
    /// </summary>
    public partial class UpbeatMainWindow : Window
    {
        /// <summary>
        /// Identifies the <see cref="Fullscreen"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty FullscreenProperty =
            DependencyProperty.Register(
                "Fullscreen",
                typeof(bool),
                typeof(UpbeatMainWindow),
                new FrameworkPropertyMetadata(false, HandleFullscreenPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="FullscreenContentMargin"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty FullscreenContentMarginProperty =
            DependencyProperty.Register(
                "FullscreenContentMargin",
                typeof(Thickness),
                typeof(UpbeatMainWindow),
                new FrameworkPropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Identifies the <see cref="ModalBackground"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty ModalBackgroundProperty =
            ModalPanel.ModalBackgroundProperty.AddOwner(typeof(UpbeatMainWindow));

        /// <summary>
        /// Identifies the <see cref="ModalBlurEffect"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty ModalBlurEffectProprety =
            DependencyProperty.Register(
                "ModalBlurEffect",
                typeof(BlurEffect),
                typeof(UpbeatMainWindow),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OverlayDataContext"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty OverlayDataContextProperty =
            DependencyProperty.Register(
                "OverlayDataContext",
                typeof(object),
                typeof(UpbeatMainWindow),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Initializes a new <see cref="UpbeatMainWindow"/>.
        /// </summary>
        public UpbeatMainWindow() =>
            InitializeComponent();

        /// <summary>
        /// Gets or sets whether the <see cref="UpbeatMainWindow"/> is in fullscreen mode or not.
        /// </summary>
        public bool Fullscreen
        {
            get => (bool)GetValue(FullscreenProperty);
            set => SetValue(FullscreenProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="UpbeatMainWindow"/>'s content margin when in fullscreen mode.
        /// <para>Under certain <see cref="Window"/> styling conditions and when in fullscreen mode, the content might be rendered outside the visible area of the screen. This property allows you to counteract that behavior by adding margin to the content to shrink its rendered size (or subtracting margin to increase the rendered size).</para>
        /// <para>This property has no effect when in windowed (non-fullscreen) mode.</para>
        /// </summary>
        public Thickness FullscreenContentMargin
        {
            get => (Thickness)GetValue(FullscreenContentMarginProperty);
            set => SetValue(FullscreenContentMarginProperty, value);
        }

        /// <summary>
        /// Gets or sets a <see cref="Brush"/> that the <see cref="ModalPanel"/> will show underneath the top (active) Element.
        /// </summary>
        public Brush ModalBackground
        {
            get => (Brush)GetValue(ModalBackgroundProperty);
            set => SetValue(ModalBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets a <see cref="BlurEffect"/> that the <see cref="ModalPanel"/> will apply to underneath (non-active) Elements.
        /// </summary>
        public BlurEffect ModalBlurEffect
        {
            get => (BlurEffect)GetValue(ModalBlurEffectProprety);
            set => SetValue(ModalBlurEffectProprety, value);
        }

        /// <summary>
        /// Gets or sets a the data context for the <see cref="UpbeatMainWindow"/>'s optional overlay element.
        /// </summary>
        public object OverlayDataContext
        {
            get => GetValue(OverlayDataContextProperty);
            set => SetValue(OverlayDataContextProperty, value);
        }

        private static void HandleFullscreenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UpbeatMainWindow upbeatMainWindow)
            {
                if (upbeatMainWindow.Fullscreen)
                {
                    upbeatMainWindow.ResizeMode = ResizeMode.NoResize;
                    upbeatMainWindow.WindowStyle = WindowStyle.None;
                    upbeatMainWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    upbeatMainWindow.WindowState = WindowState.Normal;
                    upbeatMainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                    upbeatMainWindow.ResizeMode = ResizeMode.CanResize;
                }
            }
        }
    }
}
