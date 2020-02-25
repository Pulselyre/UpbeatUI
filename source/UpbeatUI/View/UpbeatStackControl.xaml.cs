/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UpbeatUI.View
{
    public partial class UpbeatStackControl : UserControl
    {
        public static DependencyProperty BlurColorProperty =
            BlurredZPanel.BlurColorProperty.AddOwner(typeof(UpbeatStackControl));
        public static DependencyProperty BlurRadiusProperty =
            BlurredZPanel.BlurRadiusProperty.AddOwner(typeof(UpbeatStackControl));

        public UpbeatStackControl() =>
            InitializeComponent();

        public Brush BlurColor
        {
            get => (Brush)GetValue(BlurColorProperty);
            set => SetValue(BlurColorProperty, value);
        }

        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }
    }
}
