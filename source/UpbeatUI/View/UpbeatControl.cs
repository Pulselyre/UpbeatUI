/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System.Windows;
using System.Windows.Controls;

namespace UpbeatUI.View
{
    /// <summary>
    /// Provides a base contorl class that can position content within a region of its viewable space. This is necessary so the user can see and access the blurred area in the UpbeatStack to close the top control.
    /// </summary>
    public class UpbeatControl : ContentControl
    {
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage of available height that the content should fill. Can provide one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that height percentage.</para>
        /// <para>If two values are provided, the content fill its desired height between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static DependencyProperty HeightPercentProperty =
            DependencyProperty.Register(
                "HeightPercent",
                typeof(string),
                typeof(UpbeatControl),
                new PropertyMetadata(null));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing whether the content should be kept within the viewable space or not, if the XPositionPercent of YPositionPercent properties are set.
        /// </summary>
        public static DependencyProperty KeepInBoundsProperty =
            DependencyProperty.Register(
                "KeepInBounds",
                typeof(bool),
                typeof(UpbeatControl),
                new PropertyMetadata(false));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage of available width that the content should fill. Can provide one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that width percentage.</para>
        /// <para>If two values are provided, the content fill its desired width between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static DependencyProperty WidthPercentProperty =
            DependencyProperty.Register(
                "WidthPercent",
                typeof(string),
                typeof(UpbeatControl),
                new PropertyMetadata(null));

        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage point horizontally within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0. Default value is 50% (horizontally centered).
        /// </summary>
        public static DependencyProperty XPositionPercentProperty =
            DependencyProperty.Register(
                "XPositionPercent",
                typeof(object),
                typeof(UpbeatControl),
                new PropertyMetadata(0.5));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage point vertically within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0. Default value is 50% (vertically centered).
        /// </summary>
        public static DependencyProperty YPositionPercentProperty =
            DependencyProperty.Register(
                "YPositionPercent",
                typeof(object),
                typeof(UpbeatControl),
                new PropertyMetadata(0.5));

        static UpbeatControl() =>
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(UpbeatControl),
                new FrameworkPropertyMetadata(typeof(UpbeatControl)));

        /// <summary>
        /// Gets or sets the percentage of available height that the content should fill. Can provide one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that height percentage.</para>
        /// <para>If two values are provided, the content fill its desired height between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public string HeightPercent
        {
            get => (string)GetValue(HeightPercentProperty);
            set => SetValue(HeightPercentProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the content should be kept within the viewable area or not, if the XPositionPercent of YPositionPercent properties are set.
        /// </summary>
        public bool KeepInBounds
        {
            get => (bool)GetValue(KeepInBoundsProperty);
            set => SetValue(KeepInBoundsProperty, value);
        }

        /// <summary>
        /// Gets or sets the percentage of available width that the content should fill. Can provide one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that width percentage.</para>
        /// <para>If two values are provided, the content fill its desired width between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public string WidthPercent
        {
            get => (string)GetValue(WidthPercentProperty);
            set => SetValue(WidthPercentProperty, value);
        }

        /// <summary>
        /// Gets or sets the percentage point horizontally within the available space that the content should be centered on. The value can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0. Default value is 50% (horizontally centered).
        /// </summary>
        public object XPositionPercent
        {
            get => (object)GetValue(XPositionPercentProperty);
            set => SetValue(XPositionPercentProperty, value);
        }

        /// <summary>
        /// Gets or sets the percentage point vertically within the available space that the content should be centered on. The value can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0. Default value is 50% (vertically centered).
        /// </summary>
        public object YPositionPercent
        {
            get => (object)GetValue(YPositionPercentProperty);
            set => SetValue(YPositionPercentProperty, value);
        }
    }
}
