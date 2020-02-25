/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Windows;
using System.Windows.Media;

namespace UpbeatUI.View
{
    public partial class UpbeatMainWindow : Window
    {
        public UpbeatMainWindow() =>
            InitializeComponent();

        public Brush BlurColor
        {
            get => (Brush)UpbeatStack.GetValue(UpbeatStackControl.BlurColorProperty);
            set => UpbeatStack.SetValue(UpbeatStackControl.BlurColorProperty, value);
        }

        public double BlurRadius
        {
            get => (double)UpbeatStack.GetValue(UpbeatStackControl.BlurRadiusProperty);
            set => UpbeatStack.SetValue(UpbeatStackControl.BlurRadiusProperty, value);
        }
    }
}
