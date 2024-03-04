/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System.Windows;
using System.Windows.Media;

namespace UpbeatUI.View
{
    /// <summary>
    /// Defines a window with a pre-embedded BlurredZPanel based stack. The DataContext should be set to an UpbeatStack object.
    /// </summary>
    public partial class UpbeatMainWindow : Window
    {
        /// <summary>
        /// Identifies the UpbeatUI.View.UpbeatMainWindow.BlurColor dependency property.
        /// </summary>
        public readonly static DependencyProperty BlurColorProperty =
            BlurredZPanel.BlurColorProperty.AddOwner(typeof(UpbeatMainWindow));
        /// <summary>
        /// Identifies the UpbeatUI.View.UpbeatMainWindow.BlurRadius dependency property.
        /// </summary>
        public readonly static DependencyProperty BlurRadiusProperty =
            BlurredZPanel.BlurRadiusProperty.AddOwner(typeof(UpbeatMainWindow));

        /// <summary>
        /// Initializes a new UpbeatMainWindow.
        /// </summary>
        public UpbeatMainWindow() =>
            InitializeComponent();

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
    }
}
