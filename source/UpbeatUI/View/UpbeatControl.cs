/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Windows;
using System.Windows.Controls;

namespace UpbeatUI.View
{
    public class UpbeatControl : ContentControl
    {
        public static DependencyProperty HeightPercentProperty =
            DependencyProperty.Register(
                "HeightPercent",
                typeof(string),
                typeof(UpbeatControl), new PropertyMetadata(null));
        public static DependencyProperty KeepInBoundsProperty =
            DependencyProperty.Register(
                "KeepInBounds",
                typeof(bool),
                typeof(UpbeatControl),
                new PropertyMetadata(false));
        public static DependencyProperty WidthPercentProperty =
            DependencyProperty.Register(
                "WidthPercent",
                typeof(string),
                typeof(UpbeatControl), new PropertyMetadata(null));
        public static DependencyProperty XPositionPercentProperty =
            DependencyProperty.Register(
                "XPositionPercent",
                typeof(double),
                typeof(UpbeatControl),
                new PropertyMetadata(0.5));
        public static DependencyProperty YPositionPercentProperty =
            DependencyProperty.Register(
                "YPositionPercent",
                typeof(double),
                typeof(UpbeatControl),
                new PropertyMetadata(0.5));

        static UpbeatControl() =>
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(UpbeatControl),
                new FrameworkPropertyMetadata(typeof(UpbeatControl)));

        public string HeightPercent
        {
            get => (string)GetValue(HeightPercentProperty);
            set => SetValue(HeightPercentProperty, value);
        }

        public bool KeepInBounds
        {
            get => (bool)GetValue(KeepInBoundsProperty);
            set => SetValue(KeepInBoundsProperty, value);
        }

        public string WidthPercent
        {
            get => (string)GetValue(WidthPercentProperty);
            set => SetValue(WidthPercentProperty, value);
        }

        public double XPositionPercent
        {
            get => (double)GetValue(XPositionPercentProperty);
            set => SetValue(XPositionPercentProperty, value);
        }

        public double YPositionPercent
        {
            get => (double)GetValue(YPositionPercentProperty);
            set => SetValue(YPositionPercentProperty, value);
        }
    }
}
