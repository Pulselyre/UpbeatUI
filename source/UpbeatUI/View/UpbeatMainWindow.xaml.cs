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
        public readonly static DependencyProperty FullscreenProperty =
            DependencyProperty.Register(
                "Fullscreen",
                typeof(bool),
                typeof(UpbeatMainWindow),
                new FrameworkPropertyMetadata(false, HandleFullscreenPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ModalBackground"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public readonly static DependencyProperty ModalBackgroundProperty =
            ModalPanel.ModalBackgroundProperty.AddOwner(typeof(UpbeatMainWindow));

        /// <summary>
        /// Identifies the <see cref="ModalBlurEffect"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public readonly static DependencyProperty ModalBlurEffectProprety =
            DependencyProperty.Register(
                "ModalBlurEffect",
                typeof(BlurEffect),
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
