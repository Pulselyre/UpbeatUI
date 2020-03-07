/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UpbeatUI.View
{
    /// <summary>
    /// Defines a control that automatically show an appropriate Control (View) for IContexts (View Models) in a ContextStack. The DataContext should be set to a ContextStack object.
    /// </summary>
    public partial class UpbeatStackControl : UserControl
    {
        /// <summary>
        /// Identifies the UpbeatUI.View.UpbeatStackControl.BlurColor dependency property.
        /// </summary>
        public static DependencyProperty BlurColorProperty =
            BlurredZPanel.BlurColorProperty.AddOwner(typeof(UpbeatStackControl));
        /// <summary>
        /// Identifies the UpbeatUI.View.UpbeatStackControl.BlurRadius dependency property.
        /// </summary>
        public static DependencyProperty BlurRadiusProperty =
            BlurredZPanel.BlurRadiusProperty.AddOwner(typeof(UpbeatStackControl));

        /// <summary>
        /// Initializes a new UpbeatStackControl.
        /// </summary>
        public UpbeatStackControl() =>
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
