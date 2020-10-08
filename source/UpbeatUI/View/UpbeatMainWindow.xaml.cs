/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Windows;
using System.Windows.Media;

namespace UpbeatUI.View
{
    /// <summary>
    /// Defines a window with a pre-embedded UpbeatStackControl. The DataContext should be set to an UpbeatStack object.
    /// </summary>
    public partial class UpbeatMainWindow : Window
    {
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
            get => (Brush)UpbeatStack.GetValue(UpbeatStackControl.BlurColorProperty);
            set => UpbeatStack.SetValue(UpbeatStackControl.BlurColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the amount of blurring effect to show underneath the top (active) control.
        /// </summary>
        public double BlurRadius
        {
            get => (double)UpbeatStack.GetValue(UpbeatStackControl.BlurRadiusProperty);
            set => UpbeatStack.SetValue(UpbeatStackControl.BlurRadiusProperty, value);
        }
    }
}
