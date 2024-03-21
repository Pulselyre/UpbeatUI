/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UpbeatUI.View
{
    public class PercentPlaceContentControl : ContentControl
    {
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage of available height that the content should fill. Can provide one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that height percentage.</para>
        /// <para>If two values are provided, the content fill its desired height between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static readonly DependencyProperty HeightPercentProperty =
            DependencyProperty.Register(
                "HeightPercent",
                typeof(string),
                typeof(PercentPlaceContentControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing whether the content should be kept within the viewable space or not, if the XPositionPercent of YPositionPercent properties are set.
        /// </summary>
        public static readonly DependencyProperty KeepInBoundsProperty =
            DependencyProperty.Register(
                "KeepInBounds",
                typeof(bool),
                typeof(PercentPlaceContentControl),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage of available width that the content should fill. Can provide one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that width percentage.</para>
        /// <para>If two values are provided, the content fill its desired width between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static readonly DependencyProperty WidthPercentProperty =
            DependencyProperty.Register(
                "WidthPercent",
                typeof(string),
                typeof(PercentPlaceContentControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage point horizontally within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0. Default value is 50% (horizontally centered).
        /// </summary>
        public static readonly DependencyProperty XPositionPercentProperty =
            DependencyProperty.Register(
                "XPositionPercent",
                typeof(string),
                typeof(PercentPlaceContentControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage point vertically within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0. Default value is 50% (vertically centered).
        /// </summary>
        public static readonly DependencyProperty YPositionPercentProperty =
            DependencyProperty.Register(
                "YPositionPercent",
                typeof(string),
                typeof(PercentPlaceContentControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));

        private ContentPresenter _contentPresenter;

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
        public string XPositionPercent
        {
            get => (string)GetValue(XPositionPercentProperty);
            set => SetValue(XPositionPercentProperty, value);
        }

        /// <summary>
        /// Gets or sets the percentage point vertically within the available space that the content should be centered on. The value can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0. Default value is 50% (vertically centered).
        /// </summary>
        public string YPositionPercent
        {
            get => (string)GetValue(YPositionPercentProperty);
            set => SetValue(YPositionPercentProperty, value);
        }

        private ContentPresenter ContentPresenter =>
            _contentPresenter ??= VisualTreeHelper.GetChild(this, 0) as ContentPresenter;

        protected override Size MeasureOverride(Size constraint)
        {
            ContentPresenter.PercentMeasure(constraint, WidthPercent, HeightPercent);
            return new Size(
                double.IsInfinity(constraint.Width) ? ContentPresenter.DesiredSize.Width : constraint.Width,
                double.IsInfinity(constraint.Height) ? ContentPresenter.DesiredSize.Height : constraint.Height
            );
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            ContentPresenter.PercentArrange(
                arrangeBounds,
                WidthPercent,
                HeightPercent,
                XPositionPercent,
                YPositionPercent,
                KeepInBounds);
            return arrangeBounds;
        }
    }
}
