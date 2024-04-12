/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System.Windows;

namespace UpbeatUI.View
{
    /// <summary>
    /// Attached properties so elements can be arranged within a <see cref="PercentPlaceContentControl"/>  or <see cref="ModalPanel"/>.
    /// </summary>
    public static class PercentPlace
    {
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage of available height that the content should fill. Can be one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that height percentage.</para>
        /// <para>If two values are provided, the content fill its desired height between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static readonly DependencyProperty HeightPercentProperty =
            DependencyProperty.RegisterAttached(
                "HeightPercent",
                typeof(string),
                typeof(PercentPlace),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing whether the content should be kept within the viewable space or not, if the XPositionPercent of YPositionPercent properties are set.
        /// </summary>
        public static readonly DependencyProperty KeepInBoundsProperty =
            DependencyProperty.RegisterAttached(
                "KeepInBounds",
                typeof(bool?),
                typeof(PercentPlace),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage of available width that the content should fill. Can be one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that width percentage.</para>
        /// <para>If two values are provided, the content fill its desired width between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static readonly DependencyProperty WidthPercentProperty =
            DependencyProperty.RegisterAttached(
                "WidthPercent",
                typeof(string),
                typeof(PercentPlace),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage point horizontally within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as a decimal between 0.0 and 1.0. Default value is 50% (horizontally centered).
        /// </summary>
        public static readonly DependencyProperty XPositionPercentProperty =
            DependencyProperty.RegisterAttached(
                "XPositionPercent",
                typeof(string),
                typeof(PercentPlace),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));
        /// <summary>
        /// A <see cref="DependencyProperty"/> representing the percentage point vertically within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as a decimal between 0.0 and 1.0. Default value is 50% (vertically centered).
        /// </summary>
        public static readonly DependencyProperty YPositionPercentProperty =
            DependencyProperty.RegisterAttached(
                "YPositionPercent",
                typeof(string),
                typeof(PercentPlace),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets the percentage of available height that the content should fill. Can be one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that height percentage.</para>
        /// <para>If two values are provided, the content fill its desired height between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static string GetHeightPercent(DependencyObject dependencyObject) =>
            dependencyObject?.GetValue(HeightPercentProperty) as string;

        /// <summary>
        /// Sets the percentage of available height that the content should fill. Can be one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that height percentage.</para>
        /// <para>If two values are provided, the content fill its desired height between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static void SetHeightPercent(FrameworkElement frameworkElement, string value) =>
            frameworkElement?.SetValue(HeightPercentProperty, value);

        /// <summary>
        /// Gets whether the content should be kept within the viewable space or not, if the XPositionPercent of YPositionPercent properties are set.
        /// </summary>
        public static bool? GetKeepInBounds(DependencyObject dependencyObject) =>
            dependencyObject?.GetValue(KeepInBoundsProperty) as bool?;

        /// <summary>
        /// Sets whether the content should be kept within the viewable space or not, if the XPositionPercent of YPositionPercent properties are set.
        /// </summary>
        public static void SetKeepInBounds(FrameworkElement frameworkElement, bool? value) =>
            frameworkElement?.SetValue(KeepInBoundsProperty, value);

        /// <summary>
        /// Gets the percentage of available width that the content should fill. Can be one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that width percentage.</para>
        /// <para>If two values are provided, the content fill its desired width between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static string GetWidthPercent(DependencyObject dependencyObject) =>
            dependencyObject?.GetValue(WidthPercentProperty) as string;

        /// <summary>
        /// Sets the percentage of available width that the content should fill. Can be one or two values (minimum and maximum). Values can be in percent format (e.g, '50%') or as decimals between 0.0 and 1.0.
        /// <para>If one value is provided, the content will fill that width percentage.</para>
        /// <para>If two values are provided, the content fill its desired width between the first (minimum) and second (maximum) percentages.</para>
        /// </summary>
        public static void SetWidthPercent(FrameworkElement frameworkElement, string value) =>
            frameworkElement?.SetValue(WidthPercentProperty, value);

        /// <summary>
        /// Gets the percentage point horizontally within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as a decimal between 0.0 and 1.0. Default value is 50% (horizontally centered).
        /// </summary>
        public static string GetXPositionPercent(DependencyObject dependencyObject) =>
            dependencyObject?.GetValue(XPositionPercentProperty) as string;

        /// <summary>
        /// Sets the percentage point horizontally within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as a decimal between 0.0 and 1.0. Default value is 50% (horizontally centered).
        /// </summary>
        public static void SetXPositionPercent(FrameworkElement frameworkElement, string value) =>
            frameworkElement?.SetValue(XPositionPercentProperty, value);

        /// <summary>
        /// Gets the percentage point vertically within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as a decimal between 0.0 and 1.0. Default value is 50% (vertically centered).
        /// </summary>
        public static string GetYPositionPercent(DependencyObject dependencyObject) =>
            dependencyObject?.GetValue(YPositionPercentProperty) as string;

        /// <summary>
        /// Sets the percentage point vertically within the viewable space that the content should be centered on. The value can be in percent format (e.g, '50%') or as a decimal between 0.0 and 1.0. Default value is 50% (vertically centered).
        /// </summary>
        public static void SetYPositionPercent(FrameworkElement frameworkElement, string value) =>
            frameworkElement?.SetValue(YPositionPercentProperty, value);
    }
}
